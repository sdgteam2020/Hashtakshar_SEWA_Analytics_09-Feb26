async function fetchJson(url) {
    const response = await fetch(url);
    if (!response.ok) {
        throw new Error(`Request failed for ${url}: ${response.status}`);
    }
    return response.json();
}

function escapeHtml(value) {
    return String(value ?? '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function serverSideAjax(url) {
    return {
        url: url,
        type: 'POST',
        dataSrc: 'data',
        data: function (d) {
            d.searchValue = d.search && d.search.value ? d.search.value : '';
        }
    };
}

function rowNumber(meta) {
    return meta.settings._iDisplayStart + meta.row + 1;
}

async function loadDashboardCounts() {
    try {
        const applicationsEl = document.querySelector('[data-count="applications"]');
        const usersEl = document.querySelector('[data-count="users"]');
        const logsEl = document.querySelector('[data-count="logsmonitor"]');
        const vaultEl = document.querySelector('[data-count="vaultmaster"]');
        const signedEl = document.querySelector('[data-count="signed"]');

        const [totalInstallData, todayUserData, logsData, vaultData, signData] = await Promise.all([
            fetchJson('/Dashboard/TotalInstallCount'),
            fetchJson('/Dashboard/TodayUserCount'),
            fetchJson('/Dashboard/GetClientErrorLogsCount'),
            fetchJson('/Dashboard/GetVaultDataCount'),
            fetchJson('/Dashboard/GetDigitalSignCount')
        ]);

        if (applicationsEl) applicationsEl.textContent = totalInstallData.totalInstallations ?? '0';
        if (usersEl) usersEl.textContent = todayUserData.todayUsers ?? '0';
        if (logsEl) logsEl.textContent = logsData.clientErrorLogsCount ?? '0';
        if (vaultEl) vaultEl.textContent = vaultData.vaultDataCount ?? '0';
        if (signedEl) signedEl.textContent = signData.digitalSignCount ?? '0';
    } catch (error) {
        console.error('Failed to load dashboard counts.', error);
    }
}

$(function () {
    void loadDashboardCounts();
});

$(function () {
    let appsTable = null;

    $('#kpiTotalApps').on('click', function () {
        const el = document.getElementById('appsModal');

        if (el) {
            const existing = bootstrap.Modal.getInstance(el);
            if (existing) existing.dispose();

            new bootstrap.Modal(el, { backdrop: 'static', keyboard: false }).show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $('#appsModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        if (appsTable) {
            appsTable.ajax.reload(null, false);
        } else {
            appsTable = $('#tblApplications').DataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: false,
                searchDelay: 500,
                pageLength: 10,
                ajax: serverSideAjax('/Dashboard/GetApplications'),
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return rowNumber(meta);
                        }
                    },
                    { data: 'domainId', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'ipAddress', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'version', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'installDate', defaultContent: '-', render: $.fn.dataTable.render.text() }
                ],
                responsive: true,
                autoWidth: false,
                dom: '<"row mb-2"<"col-sm-6"l><"col-sm-6"f>>t<"row mt-2"<"col-sm-5"i><"col-sm-7"p>>',
                createdRow: function (row) {
                    row.classList.add('align-middle');
                }
            });
        }
    });
});

$(function () {
    let usersTable = null;

    $('#kpiTodayUsers').on('click', function () {
        const el = document.getElementById('todayUsersModal');

        if (el) {
            const existing = bootstrap.Modal.getInstance(el);
            if (existing) existing.dispose();

            new bootstrap.Modal(el, { backdrop: 'static', keyboard: false }).show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $('#todayUsersModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        if (usersTable) {
            usersTable.ajax.reload(null, false);
        } else {
            usersTable = $('#tblTodayUsers').DataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: false,
                searchDelay: 500,
                pageLength: 10,
                ajax: serverSideAjax('/Dashboard/GetTodayUsers'),
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return rowNumber(meta);
                        }
                    },
                    { data: 'domainId', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'ipAddress', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'version', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'installDate', defaultContent: '-', render: $.fn.dataTable.render.text() }
                ],
                responsive: true,
                autoWidth: false,
                dom: '<"row mb-2"<"col-sm-6"l><"col-sm-6"f>>t<"row mt-2"<"col-sm-5"i><"col-sm-7"p>>',
                createdRow: function (row) {
                    row.classList.add('align-middle');
                }
            });
        }
    });
});

$(function () {
    let logsTable = null;

    $('#kpiClientErrorLogs').on('click', function () {
        const el = document.getElementById('clientErrorLogsModal');

        if (el) {
            const existing = bootstrap.Modal.getInstance(el);
            if (existing) existing.dispose();

            new bootstrap.Modal(el, { backdrop: 'static', keyboard: false }).show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $('#clientErrorLogsModal')
        .off('shown.bs.modal')
        .on('shown.bs.modal', function () {
            if (logsTable) {
                logsTable.ajax.reload(null, false);
            } else {
                logsTable = $('#tblClientErrorLogs').DataTable({
                    processing: true,
                    serverSide: true,
                    searching: true,
                    ordering: false,
                    searchDelay: 500,
                    pageLength: 10,
                    ajax: serverSideAjax('/Dashboard/GetClientErrorLogsData'),
                    columns: [
                        {
                            data: null,
                            render: function (data, type, row, meta) {
                                return rowNumber(meta);
                            }
                        },
                        { data: 'ipAddress', defaultContent: '-', render: $.fn.dataTable.render.text() },
                        { data: 'machineName', defaultContent: '-', render: $.fn.dataTable.render.text() },
                        { data: 'userName', defaultContent: '-', render: $.fn.dataTable.render.text() },
                        { data: 'operatingSystem', defaultContent: '-', render: $.fn.dataTable.render.text() },
                        {
                            data: 'is64Bit',
                            render: function (v) {
                                return v ? 'Yes' : 'No';
                            },
                            defaultContent: '-'
                        },
                        { data: 'systemDirectory', defaultContent: '-', render: $.fn.dataTable.render.text() },
                        { data: 'appVersion', defaultContent: '-', render: $.fn.dataTable.render.text() },
                        {
                            data: 'errorMessage',
                            defaultContent: '-',
                            render: function (v, type) {
                                if (!v) return '-';

                                const value = String(v);
                                if (type !== 'display') return value;

                                const shortened = value.length > 60 ? `${value.substring(0, 60)}...` : value;
                                return escapeHtml(shortened);
                            }
                        },
                        {
                            data: 'loggedAt',
                            defaultContent: '-',
                            render: function (v, type) {
                                if (!v) return '-';
                                if (type !== 'display') return v;

                                const dt = new Date(v);
                                return Number.isNaN(dt.getTime()) ? escapeHtml(v) : escapeHtml(dt.toLocaleString());
                            }
                        },
                        {
                            data: null,
                            orderable: false,
                            searchable: false,
                            render: function (row) {
                                const safe = encodeURIComponent(JSON.stringify({
                                    errorMessage: row.errorMessage,
                                    stackTrace: row.stackTrace,
                                    extra: row.extra
                                }));

                                return `<button type="button" class="btn btn-sm btn-outline-light view-client-log" data-json="${safe}">View</button>`;
                            }
                        }
                    ],
                    responsive: true,
                    autoWidth: false,
                    scrollX: true,
                    dom: '<"row mb-2"<"col-sm-6"l><"col-sm-6"f>>t<"row mt-2"<"col-sm-5"i><"col-sm-7"p>>',
                    createdRow: function (row) {
                        row.classList.add('align-middle');
                    }
                });
            }
        });

    $(document).on('click', '.view-client-log', function () {
        const el = document.getElementById('clientErrorLogDetailModal');

        if (el) {
            const data = JSON.parse(decodeURIComponent($(this).attr('data-json')));

            $('#ced_errorMessage').text(data.errorMessage || '-');
            $('#ced_stackTrace').text(data.stackTrace || '-');
            $('#ced_extra').text(data.extra || '-');

            const existing = bootstrap.Modal.getInstance(el);
            if (existing) existing.dispose();

            new bootstrap.Modal(el, { backdrop: 'static', keyboard: true }).show();
        } else {
            console.error('Detail modal not found.');
        }
    });
});

$(function () {
    let pkvTable = null;

    $('#kpiVault').on('click', function () {
        const el = document.getElementById('vaultMasterModal');

        if (el) {
            const existing = bootstrap.Modal.getInstance(el);
            if (existing) existing.dispose();

            new bootstrap.Modal(el, { backdrop: 'static', keyboard: false }).show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $('#vaultMasterModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        if (pkvTable) {
            pkvTable.ajax.reload(null, false);
        } else {
            pkvTable = $('#tblVaultMaster').DataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: false,
                searchDelay: 500,
                pageLength: 10,
                ajax: serverSideAjax('/Dashboard/GetVaultMasterData'),
                responsive: true,
                autoWidth: false,
                scrollX: true,
                dom: '<"row mb-2"<"col-sm-6"l><"col-sm-6"f>>t<"row mt-2"<"col-sm-5"i><"col-sm-7"p>>',
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return rowNumber(meta);
                        }
                    },
                    { data: 'serialNo', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    {
                        data: 'tokenValid',
                        render: function (v) {
                            return v
                                ? '<span class="badge rounded-pill text-bg-success">Active</span>'
                                : '<span class="badge rounded-pill text-bg-danger">Inactive</span>';
                        }
                    },
                    { data: 'validFrom', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'validTo', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'createdAt', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    {
                        data: 'public_Key',
                        defaultContent: '-',
                        render: function (v, type) {
                            if (!v) return '-';
                            if (type !== 'display') return v;

                            return `<span class="text-truncate d-inline-block publicKeyStatus">${escapeHtml(v)}</span>`;
                        }
                    },
                    {
                        data: null,
                        orderable: false,
                        searchable: false,
                        render: function (row) {
                            const safeKey = encodeURIComponent(row.public_Key || '');
                            return `<button type="button" class="btn btn-sm btn-outline-light pkv-view" data-key="${safeKey}">View</button>`;
                        }
                    }
                ],
                createdRow: function (row) {
                    row.classList.add('align-middle');
                }
            });
        }
    });

    $(document).on('click', '.pkv-view', function () {
        const key = decodeURIComponent($(this).attr('data-key') || '');
        $('#pkv_keyText').val(key || '-');

        const el = document.getElementById('publicKeyViewModal');

        if (el) {
            const existing = bootstrap.Modal.getInstance(el);
            if (existing) existing.dispose();

            new bootstrap.Modal(el, { backdrop: 'static', keyboard: true }).show();
        } else {
            console.error('Public key view modal not found.');
        }
    });

    $('#btnCopyPublicKey').on('click', async function () {
        const text = $('#pkv_keyText').val() || '';
        const ta = document.getElementById('pkv_keyText');

        try {
            await navigator.clipboard.writeText(text);
        } catch (error) {
            if (ta) {
                ta.focus();
                ta.select();
            }
            console.error('Clipboard copy failed. User can press Ctrl+C manually.', error);
        }
    });
});

$(function () {
    $(document).on('click', '#kpiDigitalSignDetails', function () {
        const el = document.getElementById('digitalSignDetailsModal');

        if (el) {
            bootstrap.Modal.getOrCreateInstance(el, { backdrop: 'static', keyboard: false }).show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $(document).on('shown.bs.modal', '#digitalSignDetailsModal', function () {
        if ($.fn.DataTable.isDataTable('#tblDigitalSignDetails')) {
            $('#tblDigitalSignDetails').DataTable().ajax.reload(null, false);
        } else {
            $('#tblDigitalSignDetails').DataTable({
                processing: true,
                serverSide: true,
                searching: true,
                ordering: false,
                searchDelay: 500,
                pageLength: 10,
                ajax: serverSideAjax('/Dashboard/GetDigitalSignList'),
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return rowNumber(meta);
                        }
                    },
                    { data: 'userPublicDataId', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'documentName', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'signedAt', defaultContent: '-', render: $.fn.dataTable.render.text() },
                    { data: 'ipAddress', defaultContent: '-', render: $.fn.dataTable.render.text() }
                ],
                responsive: true,
                autoWidth: false,
                dom: '<"row mb-2"<"col-sm-6"l><"col-sm-6"f>>t<"row mt-2"<"col-sm-5"i><"col-sm-7"p>>',
                createdRow: function (row) {
                    row.classList.add('align-middle');
                }
            });
        }
    });
});
