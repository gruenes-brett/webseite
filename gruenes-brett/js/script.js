(() => {
  const application = Stimulus.Application.start()

  application.register("form", class extends Stimulus.Controller {
    static get targets() {
      return [ "startTime", "endTime", "fullDay", "placeAddress", "physicalSpace" ]
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
  })
})()