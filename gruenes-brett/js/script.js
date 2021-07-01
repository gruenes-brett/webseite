(() => {
  const application = Stimulus.Application.start()

  application.register("form", class extends Stimulus.Controller {
    static get targets() {
      return [
        "startTime", "endTime", "placeAddress", "physicalSpace", "image", "imagePreview", "imageUrl",
        "importFacebookEvent", "title", "startDate", "endDate", "description", "url"
      ];
    }

    initialize() {
      this.setAddressState()
    }

    setAddressState() {
      this.placeAddressTarget.disabled = this.physicalSpaceTarget.checked;
      if (this.physicalSpaceTarget.checked) {
        this.placeAddressTarget.value = '';
      }
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

    importFacebookEvent() {
      var url = prompt('Bitte die vollständige Addresse der Facebook-Veranstaltung eingeben:');
      if (url === null) {
        return;
      }

      fetch(wp_api.root + "comcal/v1/import-event-url?url=" + url, {
        method: "GET",
        headers: {
            "X-WP-Nonce": wp_api.nonce
        }
      })
      .then(response => response.json())
      .then(data => {
        console.log(data);
        if (data.data !== undefined && data.data.status !== 200) {
          throw Error(data.message);
        }

        // Update form controls with data from Facebook event:
        this.titleTarget.value = data.title;
        this.startTimeTarget.value = data.time;
        this.endTimeTarget.value = data.timeEnd;
        this.startDateTarget.value = data.date;
        this.endDateTarget.value = data.dateEnd;
        this.placeAddressTarget.value = data.location;
        this.descriptionTarget.value = data.description;
        this.urlTarget.value = data.url;
      })
      .catch(error => {
        alert(error);
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