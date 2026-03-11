(function () {
    const el = document.getElementById("today");
    if (!el) return;

    const d = new Date();
    el.textContent = d.toLocaleDateString('en-GB', {
        day: '2-digit',
        month: 'short',
        year: 'numeric'
    });
})();

toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: "toast-top-right",
    timeOut: 3000
};

$(function () {
    const el = document.getElementById("toastData");
    if (!el) return;

    const success = el?.getAttribute("data-success");
    const error = el?.getAttribute("data-error");
    const warning = el?.getAttribute("data-warning");
    const info = el?.getAttribute("data-info");

    if (success) toastr.success(success);
    if (error) toastr.error(error);
    if (warning) toastr.warning(warning);
    if (info) toastr.info(info);
});

(function () {
    let shown = false;

    globalThis.handleSessionExpired = function (msg) {
        if (shown) return;
        shown = true;

        try {
            $.fn.dataTable.ext.errMode = 'none';
        } catch { }

        try {
            $.fn.dataTable.tables({ visible: true, api: true }).clear().draw();
        } catch { }

        document.querySelectorAll('.modal.show').forEach(m => {
            try {
                bootstrap.Modal.getInstance(m)?.hide();
            } catch { }
        });

        document.querySelectorAll('.modal-backdrop').forEach(b => b.remove());
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('padding-right');

        const modalEl = document.getElementById('sessionExpiredModal');
        if (modalEl) {
            const m = bootstrap.Modal.getOrCreateInstance(modalEl, {
                backdrop: 'static',
                keyboard: false
            });
            m.show();

            const btn = document.getElementById('btnGoLogin');
            if (btn) {
                btn.onclick = function () {
                    globalThis.location.href = '/Auth/Login?reason=expired';
                };
            }
            return;
        }

        alert(msg || 'Session expired. Please login again.');
        globalThis.location.href = '/Auth/Login?reason=expired';
    };
})();

(function () {
    const _fetch = globalThis.fetch;

    globalThis.fetch = async function (input, init = {}) {
        init.headers = init.headers || {};

        if (!init.headers['X-Requested-With']) {
            init.headers['X-Requested-With'] = 'XMLHttpRequest';
        }

        const res = await _fetch(input, init);

        if (res.status === 401) {
            let msg = 'Session expired. Please login again.';
            try {
                const ct = res.headers.get('content-type') || '';
                if (ct.includes('application/json')) {
                    const msg = xhr?.responseJSON?.message || 'Session expired. Please login again.';
                }
            } catch { }

            globalThis.handleSessionExpired(msg);
            throw new Error('Unauthorized (401)');
        }

        const ct = res.headers.get('content-type') || '';
        if (ct.includes('text/html')) {
            globalThis.handleSessionExpired('Session expired. Please login again.');
        }

        return res;
    };
})();

try {
    if ($.fn?.dataTable) {
        $.fn.dataTable.ext.errMode = 'none';

        $(document).on('xhr.dt', function (e, settings, json, xhr) {
            if (xhr?.status === 401) {
                globalThis.handleSessionExpired(json?.message || 'Session expired. Please login again.');
            }
        });
    }
} catch { }

$(document).on('xhr.dt', function (e, settings, json, xhr) {
    if (xhr?.status === 401) {
        globalThis.handleSessionExpired(json?.message || 'Session expired. Please login again.');
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

            globalThis.handleSessionExpired(msg);
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