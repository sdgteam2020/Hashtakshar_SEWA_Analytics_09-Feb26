document.addEventListener("DOMContentLoaded", function () {

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
     
    const regUsernameInput = document.querySelector('input[name="Username"]');

    let lastToastAt = 0;
    function toastOnce(msg) {
        if (!window.toastr) return;
        const now = Date.now();
        if (now - lastToastAt < 1200) return;  
        lastToastAt = now;
        toastr.warning(msg);
    }

    if (regUsernameInput) {
        regUsernameInput.addEventListener("input", function () {
            const raw = this.value;
            const cleaned = raw.replace(/[^a-zA-Z0-9._-]/g, "");

            if (raw !== cleaned) {
                this.value = cleaned;
                toastOnce("Only letters, numbers and . _ - are allowed in username.");
                return;
            }
             
            const min = 4, max = 30;

            if (cleaned.length > max) {
                this.value = cleaned.slice(0, max);
                toastOnce(`Username can't be more than ${max} characters.`);
                return;
            }

            if (cleaned.length > 0 && cleaned.length < min) {
                 
                toastOnce(`Username must be at least ${min} characters.`);
            }
        });
    }
});
