const spnUniqueKey = document.getElementById("spnhdns").innerText;

const usernameInput = document.getElementById("loginUsername");

$(function () {
    var el = document.getElementById("authReason");
    if (!el) return;

    var reason = el.getAttribute("data-reason") || "";

    if (reason === "auth") {
        toastr.warning("Please login to continue.");
    } else if (reason === "denied") {
        toastr.error("Access denied.");
    }
});

function bindToggle(btnId, inputId) {
    const btn = document.getElementById(btnId);
    const input = document.getElementById(inputId);
    if (!btn || !input) return;

    btn.addEventListener("click", function () {
        const isPwd = input.type === "password";
        input.type = isPwd ? "text" : "password";

        const icon = btn.querySelector("i");
        if (icon) icon.className = isPwd ? "bi bi-eye-slash" : "bi bi-eye";
    });
}

bindToggle("togglePasswordBtn", "loginPassword");

usernameInput.addEventListener("input", function () {
    let value = this.value;

    
    value = value.replaceAll(/\W/g, "");

    
    let underscoreCount = 0;
    value = value.replaceAll(/_/g, (match) => {
        underscoreCount++;
        return underscoreCount <= 2 ? match : "";
    });

    if (this.value !== value) {
        this.value = value;
        toastr.warning("Only letters, numbers, and maximum 2 underscores are allowed.");
    }
});

document.getElementById("loginForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const form = e.target;

    const username = document.getElementById("loginUsername").value;
    const password = document.getElementById("loginPassword").value;
     
    const encryptedUsername = encryptData(username);
    const encryptedPassword = encryptData(password);
     
    const formData = new FormData(form);
     
    formData.set("Username", encryptedUsername);
    formData.set("Password", encryptedPassword);
     
    const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;

    try {
        const res = await fetch("/Auth/Login", {
            method: "POST",
            body: formData,
            credentials: "same-origin",
            headers: {
                "RequestVerificationToken": token,
                "X-Requested-With": "XMLHttpRequest",
                "Accept": "application/json"
            }
        });

        const contentType = res.headers.get("content-type") || "";
         
        const data = contentType.includes("application/json") ? await res.json() : null;

        if (!res.ok) {
            toastr.error(data?.message || "Login failed.");
            return;
        }
         
        toastr.success(data?.message || "Login successful.");

        setTimeout(() => {
            globalThis.location.href = data?.redirectUrl || "/Dashboard/Dashboard";
        }, 400);

    } catch (err) {
        toastr.error("Network error. Please try again.");
    }
});


function encryptData(plainText) {
    const secretKey = spnUniqueKey;
    if (!secretKey) return "";

    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.enc.Utf8.parse(secretKey.substring(0, 16));

    const encrypted = CryptoJS.AES.encrypt(plainText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    return encrypted.toString();
}
function decryptData(cipherText) {

    if (!cipherText) return "";

    const secretKey = spnUniqueKey;
    if (!secretKey) return "";

    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.enc.Utf8.parse(secretKey.substring(0, 16));

    cipherText = cipherText.replaceAll(/ /g, "+");

    const decrypted = CryptoJS.AES.decrypt(cipherText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    const result = decrypted.toString(CryptoJS.enc.Utf8);
    return result;
}