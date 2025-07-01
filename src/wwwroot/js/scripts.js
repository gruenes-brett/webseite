let copyToClipboard = document.querySelector("#copyToClipboard");
let permalink = document.querySelector("#permalink");
let forms = document.querySelectorAll("form");

if (copyToClipboard) {
  copyToClipboard.addEventListener("click", async () => {
    await navigator.clipboard.writeText(permalink.value);
    document.querySelector("#copyToClipboard img").src = "/img/check-fill.svg";
  });
}

if (permalink) {
  permalink.addEventListener("click", () => permalink.select());
}

if (forms) {
  forms.forEach(form => {
    form.addEventListener("submit", () => {
      let submits = document.querySelectorAll("button[type='submit']");
      submits.forEach(submit => submit.disabled = true);
    });
  });
}
