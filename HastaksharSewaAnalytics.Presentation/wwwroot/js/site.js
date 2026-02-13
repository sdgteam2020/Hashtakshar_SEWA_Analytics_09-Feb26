(function () {
    const el = document.getElementById("today");
    if (!el) return;
    const d = new Date();
    el.textContent = d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
})();

toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: "toast-top-right",
    timeOut: 3000
};

(function () {
    let shown = false;

    window.handleSessionExpired = function (msg) {
        if (shown) return;
        shown = true;
         
        try { $.fn.dataTable.ext.errMode = 'none'; } catch { }
         
        try {
            $.fn.dataTable.tables({ visible: true, api: true }).clear().draw();
        } catch { }
         
        document.querySelectorAll('.modal.show').forEach(m => {
            try { bootstrap.Modal.getInstance(m)?.hide(); } catch { }
        });
         
        document.querySelectorAll('.modal-backdrop').forEach(b => b.remove());
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('padding-right');
         
        const modalEl = document.getElementById('sessionExpiredModal');
        if (modalEl) {
            const m = bootstrap.Modal.getOrCreateInstance(modalEl, { backdrop: 'static', keyboard: false });
            m.show();

            const btn = document.getElementById('btnGoLogin');
            if (btn) {
                btn.onclick = function () {
                    window.location.href = '/Auth/Login?reason=expired';
                };
            }
            return;
        }
         
        alert(msg || 'Session expired. Please login again.');
        window.location.href = '/Auth/Login?reason=expired';
    };
})();


(function () {
    const _fetch = window.fetch;

    window.fetch = async function (input, init = {}) {
        init.headers = init.headers || {};
         
        if (!init.headers['X-Requested-With']) init.headers['X-Requested-With'] = 'XMLHttpRequest';

        // If you use antiforgery for POST via fetch, add token header here (optional)
        // init.headers['RequestVerificationToken'] = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const res = await _fetch(input, init);

        if (res.status === 401) { 
            let msg = 'Session expired. Please login again.';
            try {
                const ct = res.headers.get('content-type') || '';
                if (ct.includes('application/json')) {
                    const j = await res.clone().json();
                    if (j?.message) msg = j.message;
                }
            } catch { }
            window.handleSessionExpired(msg);
            throw new Error('Unauthorized (401)');
        }
         
        const ct = res.headers.get('content-type') || '';
        if (ct.includes('text/html')) { 
              window.handleSessionExpired('Session expired. Please login again.');
        }

        return res;
    };
})();
 
try {
    if ($.fn && $.fn.dataTable) {
        $.fn.dataTable.ext.errMode = 'none';

        $(document).on('xhr.dt', function (e, settings, json, xhr) {
            if (xhr && xhr.status === 401) {
                window.handleSessionExpired(json?.message || 'Session expired. Please login again.');
            }
        });
    }
} catch { }

 
$(document).on('xhr.dt', function (e, settings, json, xhr) {
    if (xhr && xhr.status === 401) {
        window.handleSessionExpired(json?.message || 'Session expired. Please login again.');
    }
});

$(document).on('error.dt', function (e, settings, techNote, message) {
     
});

$.ajaxSetup({
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    statusCode: {
        401: function (xhr) {
            let msg = 'Session expired. Please login again.';
            try {
                const j = xhr.responseJSON;
                if (j?.message) msg = j.message;
            } catch { }
            window.handleSessionExpired(msg);
        }
    }
});

async function ensureAuthOrShowExpired() {
    try {
        const res = await fetch('/Auth/PingAuth', {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        return res.status !== 401;
    } catch { 
        return false;
    }
}


(function () {
    const KPI_SELECTORS = [
        '#kpiTotalApps',
        '#kpiTodayUsers',
        '#kpiClientErrorLogs',
        '#kpiVault',
        '#kpiDigitalSignDetails'
    ].join(',');

    document.addEventListener('click', async function (e) {
        const kpi = e.target.closest(KPI_SELECTORS);
        if (!kpi) return;
         
        if (kpi.dataset.allowOpen === '1') return;
         
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();

        const ok = await ensureAuthOrShowExpired();
        if (!ok) return;  
         
        kpi.dataset.allowOpen = '1';
        kpi.click();
        kpi.dataset.allowOpen = '0';
    }, true); 
})();
