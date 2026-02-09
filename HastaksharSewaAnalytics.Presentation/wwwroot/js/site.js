// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Set today's date in header (auto)
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

const myHeadersIncrement = new Headers();
myHeadersIncrement.append("X-API-KEY", '26a7d85c-29c0-48c3-bffc-5fa701d09865');

const requestIncrement = {
    method: "POST",
    redirect: "follow",
    headers: myHeadersIncrement
};

fetch("https://hitcounter.army.mil/api/ApplicationHit/IncrementHits", requestIncrement)
    .then((response) => response.text())
    .then((result) => console.log(result))
    .catch((error) => console.error(error));

const myHeadersIncrementStart = new Headers();
myHeadersIncrementStart.append("X-API-KEY", '26a7d85c-29c0-48c3-bffc-5fa701d09865');
const requestStart = {
    method: "POST",
    redirect: "follow",
    headers: myHeadersIncrementStart
};

fetch("https://hitcounter.army.mil/api/Application/ApplicationSessionStart", requestStart)
    .then((response) => response.text())
    .then((result) => console.log(result))
    .catch((error) => console.error(error));

const myHeaderstall = new Headers();
myHeaderstall.append("X-API-KEY", '26a7d85c-29c0-48c3-bffc-5fa701d09865');
const requestall = {
    method: "POST",
    redirect: "follow",
    headers: myHeaderstall
};

fetch("https://hitcounter.army.mil/api/ApplicationHit/HitswithConcurrentuser", requestall)
    .then((response) => response.json())
    .then((data) => {
        console.log("todayHits:", data.TodayHits);
        console.log("monthlyHits:", data.MonthlyHits);
        console.log("totalHits:", data.TotalHits);
        console.log("concurrentuser:", data.Concurrentuser);

        // Footer binding
        const set = (id, val) => {
            const el = document.getElementById(id);
            if (el) el.textContent = val ?? 0;
        };

        set("hitToday", data.TodayHits);
        set("hitMonth", data.MonthlyHits);
        set("hitTotal", data.TotalHits);
        set("hitConcurrent", data.Concurrentuser);
    })
    .catch((error) => console.error(error));

// Dynamic Day and Month badges
(function setBadges() {
    const d = new Date();
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    const days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    const dayEl = document.getElementById("badgeDay");
    const monthEl = document.getElementById("badgeMonth");

    if (dayEl) dayEl.textContent = days[d.getDay()]; 
    if (monthEl) monthEl.textContent = months[d.getMonth()];
})();
