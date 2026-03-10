document.addEventListener("DOMContentLoaded", function () {

    const spnUniqueKey = document.getElementById("spnhdns")?.innerText?.trim();
    const registerForm = document.getElementById("registerForm");
    const regUsernameInput = document.getElementById("regUsername");
    const regPasswordInput = document.getElementById("regPassword");
    const regConfirmPasswordInput = document.getElementById("regConfirmPassword");

    const usernameError = document.getElementById("usernameError");
    const passwordError = document.getElementById("passwordError");
    const confirmPasswordError = document.getElementById("confirmPasswordError");

    let lastToastAt = 0;

    function toastOnce(msg, type = "warning") {
        if (!window.toastr) return;

        const now = Date.now();
        if (now - lastToastAt < 1200) return;

        lastToastAt = now;

        if (type === "error") toastr.error(msg);
        else if (type === "success") toastr.success(msg);
        else toastr.warning(msg);
    }

    function setFieldError(element, message) {
        if (element) {
            element.textContent = message || "";
        }
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

    function validateUsername() {
        if (!regUsernameInput) return true;

        const raw = regUsernameInput.value || "";
        const cleaned = raw.replace(/[^a-zA-Z0-9._-]/g, "");

        if (raw !== cleaned) {
            regUsernameInput.value = cleaned;
            setFieldError(usernameError, "Only letters, numbers and . _ - are allowed.");
            toastOnce("Only letters, numbers and . _ - are allowed in username.");
            return false;
        }

        const min = 4;
        const max = 30;

        if (cleaned.length > max) {
            regUsernameInput.value = cleaned.slice(0, max);
            setFieldError(usernameError, `Username can't be more than ${max} characters.`);
            toastOnce(`Username can't be more than ${max} characters.`);
            return false;
        }

        if (cleaned.length > 0 && cleaned.length < min) {
            setFieldError(usernameError, `Username must be at least ${min} characters.`);
            return false;
        }

        if (cleaned.length === 0) {
            setFieldError(usernameError, "Username is required.");
            return false;
        }

        setFieldError(usernameError, "");
        return true;
    }

    function validatePassword() {
        if (!regPasswordInput) return true;

        const password = regPasswordInput.value || "";
        const strongPassword = /^(?=.*[A-Z])(?=.*\d).{8,}$/;

        if (!password) {
            setFieldError(passwordError, "Password is required.");
            return false;
        }

        if (!strongPassword.test(password)) {
            setFieldError(passwordError, "Password must be at least 8 characters and include 1 uppercase letter and 1 number.");
            return false;
        }

        setFieldError(passwordError, "");
        return true;
    }

    function validateConfirmPassword() {
        if (!regPasswordInput || !regConfirmPasswordInput) return true;

        const password = regPasswordInput.value || "";
        const confirmPassword = regConfirmPasswordInput.value || "";

        if (!confirmPassword) {
            setFieldError(confirmPasswordError, "Confirm Password is required.");
            return false;
        }

        if (password !== confirmPassword) {
            setFieldError(confirmPasswordError, "Password and Confirm Password do not match.");
            return false;
        }

        setFieldError(confirmPasswordError, "");
        return true;
    }

    if (regUsernameInput) {
        regUsernameInput.addEventListener("input", validateUsername);
        regUsernameInput.addEventListener("blur", validateUsername);
    }

    if (regPasswordInput) {
        regPasswordInput.addEventListener("input", function () {
            validatePassword();
            validateConfirmPassword();
        });
        regPasswordInput.addEventListener("blur", validatePassword);
    }

    if (regConfirmPasswordInput) {
        regConfirmPasswordInput.addEventListener("input", validateConfirmPassword);
        regConfirmPasswordInput.addEventListener("blur", validateConfirmPassword);
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

            const isUsernameValid = validateUsername();
            const isPasswordValid = validatePassword();
            const isConfirmPasswordValid = validateConfirmPassword();

            if (!isUsernameValid || !isPasswordValid || !isConfirmPasswordValid) {
                toastOnce("Please correct the highlighted fields.", "error");
                return;
            }

            const username = regUsernameInput.value.trim();
            const password = regPasswordInput.value;
            const confirmPassword = regConfirmPasswordInput.value;

            const encryptedUsername = encryptData(username);
            const encryptedPassword = encryptData(password);
            const encryptedConfirmPassword = encryptData(confirmPassword);

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
                    window.location.href = data?.redirectUrl || "/Dashboard/Dashboard";
                }, 400);

            } catch (err) {
                toastr.error("Network error. Please try again.");
            }
        });
    }
});