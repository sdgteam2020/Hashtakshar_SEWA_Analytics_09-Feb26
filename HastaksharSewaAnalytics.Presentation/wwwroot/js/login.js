const spnUniqueKey = document.getElementById("spnhdns").innerText;

const usernameInput = document.getElementById("loginUsername");

usernameInput.addEventListener("input", function () {
    // Allow: letters, numbers, dot, underscore, @, hyphen
    const allowed = /[^a-zA-Z0-9._@-]/g;

    if (allowed.test(this.value)) {
        this.value = this.value.replace(allowed, "");
        toastr.warning("Special characters are not allowed.");
    }
});


document.getElementById("loginForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const form = e.target;

    const username = document.getElementById("loginUsername").value;
    const password = document.getElementById("loginPassword").value;

    // Encrypt
    const encryptedUsername = encryptData(username);
    const encryptedPassword = encryptData(password);

    // Build FormData FROM the form (so it includes __RequestVerificationToken)
    const formData = new FormData(form);

    // Overwrite fields with encrypted values
    formData.set("Username", encryptedUsername);
    formData.set("Password", encryptedPassword);

    // Read anti-forgery token value
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

        // Expect JSON (your controller returns JSON for AJAX)
        const data = contentType.includes("application/json") ? await res.json() : null;

        if (!res.ok) {
            toastr.error(data?.message || "Login failed.");
            return;
        }

        // success
        toastr.success(data?.message || "Login successful.");

        setTimeout(() => {
            window.location.href = data?.redirectUrl || "/Dashboard/Dashboard";
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

    cipherText = cipherText.replace(/ /g, "+");

    const decrypted = CryptoJS.AES.decrypt(cipherText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    const result = decrypted.toString(CryptoJS.enc.Utf8);
    return result;
}