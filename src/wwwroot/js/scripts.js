let copyToClipboard = document.querySelector("#copyToClipboard");
let permalink = document.querySelector("#permalink");

if (copyToClipboard) {
  copyToClipboard.addEventListener("click", async () => {
    await navigator.clipboard.writeText(permalink.value);
    document.querySelector("#copyToClipboard img").src = "/img/check-fill.svg";
  });
}

if (permalink) {
  permalink.addEventListener("click", () => permalink.select());
}
