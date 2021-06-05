(() => {
  const application = Stimulus.Application.start()

  application.register("form", class extends Stimulus.Controller {
    static get targets() {
      return [ "startTime", "endTime", "fullDay", "placeAddress", "physicalSpace", "image", "imagePreview", "imageUrl"]
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
      formData.append('image', input.files[0]);

      fetch(wp_api.root + 'gruenesbrett/v1/upload', {
        method: 'POST',
        headers: {
            'X-WP-Nonce': wp_api.nonce
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
        console.error('Error:', error);
      });
    }
  })
})()