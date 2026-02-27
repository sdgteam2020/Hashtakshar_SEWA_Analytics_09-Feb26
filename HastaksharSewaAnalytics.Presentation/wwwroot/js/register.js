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

    if (regUsernameInput) {
        regUsernameInput.addEventListener("input", function () { 
            const allowed = /[^a-zA-Z0-9._-]/g;

            if (allowed.test(this.value)) {
                this.value = this.value.replace(allowed, "");
                if (window.toastr) {
                    toastr.warning("Special characters are not allowed in username.");
                }
            }
        });
    }
});
