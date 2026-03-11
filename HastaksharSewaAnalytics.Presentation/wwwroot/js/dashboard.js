async function fetchJson(url) {
    const response = await fetch(url);
    if (!response.ok) {
        throw new Error(`Request failed for ${url}: ${response.status}`);
    }
    return response.json();
}

async function loadDashboardCounts() {
    try {
        const applicationsEl = document.querySelector('[data-count="applications"]');
        const usersEl = document.querySelector('[data-count="users"]');
        const logsEl = document.querySelector('[data-count="logsmonitor"]');
        const vaultEl = document.querySelector('[data-count="vaultmaster"]');
        const signedEl = document.querySelector('[data-count="signed"]');

        const [
            totalInstallData,
            todayUserData,
            logsData,
            vaultData,
            signData
        ] = await Promise.all([
            fetchJson('/Dashboard/TotalInstallCount'),
            fetchJson('/Dashboard/TodayUserCount'),
            fetchJson('/Dashboard/GetClientErrorLogsCount'),
            fetchJson('/Dashboard/GetVaultDataCount'),
            fetchJson('/Dashboard/GetDigitalSignCount')
        ]);

        if (applicationsEl) {
            applicationsEl.textContent = totalInstallData.totalInstallations ?? '0';
        }

        if (usersEl) {
            usersEl.textContent = todayUserData.todayUsers ?? '0';
        }

        if (logsEl) {
            logsEl.textContent = logsData.clientErrorLogsCount ?? '0';
        }

        if (vaultEl) {
            vaultEl.textContent = vaultData.vaultDataCount ?? '0';
        }

        if (signedEl) {
            signedEl.textContent = signData.digitalSignCount ?? '0';
        }
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
            if (existing) {
                existing.dispose();
            }

            const modal = new bootstrap.Modal(el, { backdrop: 'static', keyboard: false });
            modal.show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $('#appsModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        if (appsTable) {
            appsTable.ajax.reload(null, false);
        } else {
            appsTable = $('#tblApplications').DataTable({
                ajax: { url: '/Dashboard/GetApplications', type: 'GET', dataSrc: '' },
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return meta.row + 1;
                        }
                    },
                    { data: 'domainId' },
                    { data: 'ipAddress' },
                    { data: 'version' },
                    { data: 'installDate' }
                ],
                pageLength: 10,
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
            if (existing) {
                existing.dispose();
            }

            const modal = new bootstrap.Modal(el, { backdrop: 'static', keyboard: false });
            modal.show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $('#todayUsersModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        if (usersTable) {
            usersTable.ajax.reload(null, false);
        } else {
            usersTable = $('#tblTodayUsers').DataTable({
                ajax: { url: '/Dashboard/GetTodayUsers', type: 'GET', dataSrc: '' },
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return meta.row + 1;
                        }
                    },
                    { data: 'domainId' },
                    { data: 'ipAddress' },
                    { data: 'version' },
                    { data: 'installDate' }
                ],
                pageLength: 10,
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
            if (existing) {
                existing.dispose();
            }

            const modal = new bootstrap.Modal(el, { backdrop: 'static', keyboard: false });
            modal.show();
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
                    ajax: { url: '/Dashboard/getclienterrorlogsdata', type: 'GET', dataSrc: '' },
                    columns: [
                        {
                            data: null,
                            render: function (data, type, row, meta) {
                                return meta.row + 1;
                            }
                        },
                        { data: 'ipAddress', defaultContent: '-' },
                        { data: 'machineName', defaultContent: '-' },
                        { data: 'userName', defaultContent: '-' },
                        { data: 'operatingSystem', defaultContent: '-' },
                        {
                            data: 'is64Bit',
                            render: function (v) {
                                return v ? 'Yes' : 'No';
                            },
                            defaultContent: '-'
                        },
                        { data: 'systemDirectory', defaultContent: '-' },
                        { data: 'appVersion', defaultContent: '-' },
                        {
                            data: 'errorMessage',
                            defaultContent: '-',
                            render: function (v) {
                                if (!v) {
                                    return '-';
                                }
                                return v.length > 60 ? `${v.substring(0, 60)}...` : v;
                            }
                        },
                        {
                            data: 'loggedAt',
                            defaultContent: '-',
                            render: function (v) {
                                if (!v) {
                                    return '-';
                                }

                                const dt = new Date(v);
                                return Number.isNaN(dt.getTime()) ? v : dt.toLocaleString();
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
                    pageLength: 10,
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
            if (existing) {
                existing.dispose();
            }

            const modal = new bootstrap.Modal(el, { backdrop: 'static', keyboard: true });
            modal.show();
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
            if (existing) {
                existing.dispose();
            }

            const modal = new bootstrap.Modal(el, { backdrop: 'static', keyboard: false });
            modal.show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $('#vaultMasterModal').off('shown.bs.modal').on('shown.bs.modal', function () {
        if (pkvTable) {
            pkvTable.ajax.reload(null, false);
        } else {
            pkvTable = $('#tblVaultMaster').DataTable({
                ajax: { url: '/Dashboard/GetVaultMasterData', type: 'GET', dataSrc: '' },
                pageLength: 10,
                responsive: true,
                autoWidth: false,
                scrollX: true,
                dom: '<"row mb-2"<"col-sm-6"l><"col-sm-6"f>>t<"row mt-2"<"col-sm-5"i><"col-sm-7"p>>',
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return meta.row + 1;
                        }
                    },
                    {
                        data: 'serialNo',
                        defaultContent: '-'
                    },
                    {
                        data: 'tokenValid',
                        render: function (v) {
                            return v
                                ? '<span class="badge rounded-pill text-bg-success">Active</span>'
                                : '<span class="badge rounded-pill text-bg-danger">Inactive</span>';
                        }
                    },
                    { data: 'validFrom', defaultContent: '-' },
                    { data: 'validTo', defaultContent: '-' },
                    { data: 'createdAt', defaultContent: '-' },
                    {
                        data: 'public_Key',
                        defaultContent: '-',
                        render: function (v) {
                            if (!v) {
                                return '-';
                            }
                            return `<span class="text-truncate d-inline-block publicKeyStatus">${v}</span>`;
                        }
                    },
                    {
                        data: null,
                        orderable: false,
                        searchable: false,
                        render: function (row) {
                            const safeKey = encodeURIComponent(row.public_Key || '');
                            return `
                                <button type="button" class="btn btn-sm btn-outline-light pkv-view" data-key="${safeKey}">
                                    View
                                </button>
                            `;
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
            if (existing) {
                existing.dispose();
            }

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
            const modal = bootstrap.Modal.getOrCreateInstance(el, { backdrop: 'static', keyboard: false });
            modal.show();
        } else {
            console.error('Modal not found. Render the partial first.');
        }
    });

    $(document).on('shown.bs.modal', '#digitalSignDetailsModal', function () {
        if ($.fn.DataTable.isDataTable('#tblDigitalSignDetails')) {
            $('#tblDigitalSignDetails').DataTable().ajax.reload(null, false);
        } else {
            $('#tblDigitalSignDetails').DataTable({
                ajax: { url: '/Dashboard/GetDigitalSignList', type: 'GET', dataSrc: '' },
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return meta.row + 1;
                        }
                    },
                    { data: 'userPublicDataId' },
                    { data: 'documentName' },
                    { data: 'signedAt' },
                    { data: 'ipAddress' }
                ],
                pageLength: 10,
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