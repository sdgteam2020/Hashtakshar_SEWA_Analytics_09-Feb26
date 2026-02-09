new Chart(document.getElementById('signTrendChart'), {
    type: 'line',
    data: {
        labels: ['Jan 13', 'Jan 14', 'Jan 15', 'Jan 16', 'Jan 17', 'Jan 18', 'Jan 19'],
        datasets: [{
            label: 'Documents Signed',
            data: [2, 5, 7, 6, 9, 12, 15],
            borderWidth: 2,
            fill: true,
            tension: 0.4
        }]
    },
    options: {
        plugins: { legend: { labels: { color: '#c7ffd8' } } },
        scales: {
            x: { ticks: { color: '#c7ffd8' } },
            y: { ticks: { color: '#c7ffd8' } }
        }
    }
});


new Chart(document.getElementById('usersByStateChart'), {
    type: 'bar',
    data: {
        labels: ['Delhi', 'UP', 'MH', 'TN', 'WB'],
        datasets: [{
            label: 'Users',
            data: [15, 22, 18, 10, 8]
        }]
    },
    options: {
        plugins: { legend: { labels: { color: '#c7ffd8' } } },
        scales: {
            x: { ticks: { color: '#c7ffd8' } },
            y: { ticks: { color: '#c7ffd8' } }
        }
    }
});

new Chart(document.getElementById('vaultUsageChart'), {
    type: 'doughnut',
    data: {
        labels: ['Active', 'Archived', 'Expired'],
        datasets: [{
            data: [12, 5, 2]
        }]
    },
    options: {
        plugins: { legend: { labels: { color: '#c7ffd8' } } }
    }
});

new Chart(document.getElementById('logsChart'), {
    type: 'line',
    data: {
        labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
        datasets: [{
            label: 'Logs',
            data: [50, 70, 65, 90, 120, 110, 130],
            fill: true,
            tension: 0.3
        }]
    },
    options: {
        plugins: { legend: { labels: { color: '#c7ffd8' } } },
        scales: {
            x: { ticks: { color: '#c7ffd8' } },
            y: { ticks: { color: '#c7ffd8' } }
        }
    }
});
