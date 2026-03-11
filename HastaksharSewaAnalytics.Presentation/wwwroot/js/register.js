document.addEventListener("DOMContentLoaded", function () {

    const spnUniqueKey = document.getElementById("spnhdns")?.innerText?.trim();
    const registerForm = document.getElementById("registerForm");
    const regUsernameInput = document.getElementById("regUsername");
    const regPasswordInput = document.getElementById("regPassword");
    const regConfirmPasswordInput = document.getElementById("regConfirmPassword");

    let lastToastAt = 0;

    function toastOnce(msg, type = "warning") {
        if (!globalThis.toastr || !msg) return;

        const now = Date.now();
        if (now - lastToastAt < 800) return;

        lastToastAt = now;

        if (type === "error") toastr.error(msg);
        else if (type === "success") toastr.success(msg);
        else if (type === "info") toastr.info(msg);
        else toastr.warning(msg);
    }

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

    bindToggle("toggleRegPwd", "regPassword");
    bindToggle("toggleRegConfirmPwd", "regConfirmPassword");

    function validateUsername(showToast = false) {
        if (!regUsernameInput) {
            return { valid: true, message: "" };
        }

        const raw = regUsernameInput.value || "";
         
        let cleaned = raw.replaceAll(/\W/g, "");
         
        let underscoreCount = 0;
        cleaned = cleaned.replaceAll(/_/g, () => {
            underscoreCount++;
            return underscoreCount <= 2 ? "_" : "";
        });

        if (raw !== cleaned) {
            regUsernameInput.value = cleaned;
            const msg = "Only letters, numbers, and maximum 2 underscores (_) are allowed in username.";
            if (showToast) {
                toastOnce(msg, "warning");
            }
            return { valid: false, message: msg };
        }

        const min = 4;
        const max = 15;

        if (cleaned.length === 0) {
            return { valid: false, message: "Username is required." };
        }

        if (cleaned.length < min) {
            return { valid: false, message: `Username must be at least ${min} characters.` };
        }

        if (cleaned.length > max) {
            regUsernameInput.value = cleaned.slice(0, max);
            return { valid: false, message: `Username can't be more than ${max} characters.` };
        }

        if (!/[A-Za-z]/.test(cleaned)) {
            return { valid: false, message: "Username must contain at least one letter." };
        }

        if (!/\d/.test(cleaned)) {
            return { valid: false, message: "Username must contain at least one number." };
        }

        return { valid: true, message: "" };
    }

    function validatePassword(showToast = false) {
        if (!regPasswordInput) return { valid: true, message: "" };

        const password = regPasswordInput.value || "";
        const strongPassword = /^(?=.*[A-Z])(?=.*\d).{8,}$/;

        if (!password) {
            return { valid: false, message: "Password is required." };
        }

        if (!strongPassword.test(password)) {
            return {
                valid: false,
                message: "Password must be at least 8 characters and include 1 uppercase letter and 1 number."
            };
        }

        return { valid: true, message: "" };
    }

    function validateConfirmPassword(showToast = false) {
        if (!regPasswordInput || !regConfirmPasswordInput) return { valid: true, message: "" };

        const password = regPasswordInput.value || "";
        const confirmPassword = regConfirmPasswordInput.value || "";

        if (!confirmPassword) {
            return { valid: false, message: "Confirm Password is required." };
        }

        if (password !== confirmPassword) {
            return { valid: false, message: "Password and Confirm Password do not match." };
        }

        return { valid: true, message: "" };
    }

    if (regUsernameInput) {
        regUsernameInput.addEventListener("input", function () {
            const result = validateUsername(false);
            if (!result.valid && regUsernameInput.value.length > 0) {
                if (!result.valid) toastOnce(result.message, "warning");
            }
        });

        regUsernameInput.addEventListener("blur", function () {
            const result = validateUsername(true);
            if (!result.valid) toastOnce(result.message, "warning");
        });
    }

    if (regPasswordInput) {
        regPasswordInput.addEventListener("blur", function () {
            const result = validatePassword(true);
            if (!result.valid) toastOnce(result.message, "warning");
        });

        regPasswordInput.addEventListener("input", function () {       
            validateConfirmPassword(false);
        });
    }

    if (regConfirmPasswordInput) {
        regConfirmPasswordInput.addEventListener("blur", function () {
            const result = validateConfirmPassword(true);
            if (!result.valid) toastOnce(result.message, "warning");
        });
    }

    function encryptData(plainText) {
        if (!plainText) return "";
        if (!spnUniqueKey) return "";

        const key = CryptoJS.enc.Utf8.parse(spnUniqueKey);
        const iv = CryptoJS.enc.Utf8.parse(spnUniqueKey.substring(0, 16));

        const encrypted = CryptoJS.AES.encrypt(plainText, key, {
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });

        return encrypted.toString();
    }

    if (registerForm) {
        registerForm.addEventListener("submit", async function (e) {
            e.preventDefault();

            const usernameResult = validateUsername(false);
            const passwordResult = validatePassword(false);
            const confirmPasswordResult = validateConfirmPassword(false);

            const errors = [];

            if (!usernameResult.valid) errors.push(usernameResult.message);
            if (!passwordResult.valid) errors.push(passwordResult.message);
            if (!confirmPasswordResult.valid) errors.push(confirmPasswordResult.message);

            if (errors.length > 0) {
                const uniqueErrors = [...new Set(errors)];
                uniqueErrors.forEach(msg => toastr.warning(msg));
                return;
            }

            const username = regUsernameInput.value.trim();
            const password = regPasswordInput.value;
            const confirmPassword = regConfirmPasswordInput.value;

            const encryptedUsername = encryptData(username);
            const encryptedPassword = encryptData(password);
            const encryptedConfirmPassword = encryptData(confirmPassword);

            if (!encryptedUsername || !encryptedPassword || !encryptedConfirmPassword) {
                toastr.error("Security error occurred. Please refresh the page and try again.");
                return;
            }

            const formData = new FormData(registerForm);
            formData.set("Username", encryptedUsername);
            formData.set("Password", encryptedPassword);
            formData.set("ConfirmPassword", encryptedConfirmPassword);

            const token = registerForm.querySelector('input[name="__RequestVerificationToken"]')?.value;

            try {
                const res = await fetch("/Auth/Register", {
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
                    toastr.error(data?.message || "Registration failed.");
                    return;
                }

                toastr.success(data?.message || "Registration successful.");

                setTimeout(() => {
                    globalThis.location.href = data?.redirectUrl || "/Dashboard/Dashboard";
                }, 400);

            } catch (err) {
                toastr.error("Network error. Please try again.");
            }
        });
    }
});