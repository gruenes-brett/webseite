(() => {
  const application = Stimulus.Application.start()

  application.register("form", class extends Stimulus.Controller {
    static get targets() {
      return ["startTime", "endTime", "fullDay", "placeAddress", "physicalSpace", "image", "imagePreview", "imageUrl"]
    }

    initialize() {
      this.setTimeState()
      this.setAddressState()
    }

    setTimeState() {
      this.startTimeTarget.disabled = this.fullDayTarget.checked;
      this.endTimeTarget.disabled = this.fullDayTarget.checked;
    }

    setAddressState() {
      this.placeAddressTarget.disabled = this.physicalSpaceTarget.checked;
    }

    uploadImage() {
      var input = this.imageTarget;
      var preview = this.imagePreviewTarget;
      var imageUrl = this.imageUrlTarget;

      preview.classList.add("loading");

      const formData = new FormData();
      formData.append("image", input.files[0]);

      fetch(wp_api.root + "gruenesbrett/v1/upload", {
        method: "POST",
        headers: {
            "X-WP-Nonce": wp_api.nonce
        },
        body: formData
      })
      .then(response => {
        preview.classList.remove("loading");
        return response.json()
      })
      .then(result => {
        preview.style.backgroundImage = "url(" + result + ")";
        imageUrl.value = result;
      })
      .catch(error => {
        console.error("Error:", error);
      });
    }
  })

  application.register("share", class extends Stimulus.Controller {
    static get targets() {
      return ["permalink", "clipboard", "clipboardIcon", "facebook", "twitter", "telegram", "calendar"]
    }

    copyPermalink() {
      this.permalinkTarget.select();
      document.execCommand("copy");
      window.getSelection().removeAllRanges();
      this.clipboardIconTarget.src = this.clipboardIconTarget.src.replace("clipboard-fill", "check-fill");
    }

    onFacebook() {
      var url = "https://facebook.com/sharer.php?u=" + this.permalinkTarget.value;
      window.open(url);
    }

    onTwitter() {
      var url = "https://twitter.com/intent/tweet?url=" + this.permalinkTarget.value;
      window.open(url);
    }

    onTelegram() {
      var url = "https://t.me/share/url?url=" + this.permalinkTarget.value;
      window.open(url);
    }

    withCalendar() {
      // TODO: download ical file
    }
  })
})()