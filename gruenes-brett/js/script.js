(() => {
  const application = Stimulus.Application.start()

  application.register("form", class extends Stimulus.Controller {
    static get targets() {
      return [
        "startTime", "endTime", "placeAddress", "placeName", "physicalSpace", "image", "imagePreview", "imageUrl",
        "importEvent", "title", "startDate", "endDate", "description", "url", "table", "organizer"
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

    importEvent() {
      var url = prompt('Bitte die vollständige Adresse der Veranstaltung eingeben:');
      if (url === null) {
        return;
      }

      var table = this.tableTarget;
      table.classList.add('loading');
      var apiUrl = wp_api.root + 'comcal/v1/import-event-url';
      apiUrl += (apiUrl.indexOf('?') > 0) ? '&' : '?';
      apiUrl += 'url=' + encodeURIComponent(url);
      fetch(apiUrl, {
        method: "GET",
        headers: {
            "X-WP-Nonce": wp_api.nonce
        }
      })
      .then(response => {
        table.classList.remove("loading");
        return response.json()
      })
      .then(data => {
        console.log(data);
        if (data.data !== undefined && data.data.status !== 200) {
          throw Error(data.message);
        }

        // Update form controls with data from event:
        this.titleTarget.value = data.title;
        this.startTimeTarget.value = data.time;
        this.endTimeTarget.value = data.timeEnd;
        this.startDateTarget.value = data.date;
        this.endDateTarget.value = data.dateEnd;
        this.placeAddressTarget.value = data.address;
        this.placeNameTarget.value = data.location;
        this.descriptionTarget.value = data.description;
        this.organizerTarget.value = data.organizer;
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
  })

  application.register("categories", class extends Stimulus.Controller {
    initialize() {
      Coloris({
        el: ".coloris",
        swatches: [],
        alpha: false
      });
    }

    colorUpdated(event) {
      var style_input = document.getElementById(event.currentTarget.dataset.update);
      var values = style_input.value.split(",");
      if (event.currentTarget.dataset.role === "text") {
        values[1] = event.currentTarget.value;
      } else if (event.currentTarget.dataset.role === "background") {
        values[0] = event.currentTarget.value;
      }
      style_input.value = values.join(",");
    }
  })
})()
