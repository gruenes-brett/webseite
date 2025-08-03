const postCodeInput = document.querySelector("#postCode");

if (postCodeInput) {
  const postCodeAutocomplete = new autoComplete({
    selector: "#postCode",
    data: {
      src: async (query) => {
        try {
          const source = await fetch(`/api/postcodes?query=${query}`);
          return await source.json();
        } catch (error) {
          return error;
        }
      }
    },
    debounce: 200,
    resultItem: {
      highlight: true,
    },
    resultsList: {
      maxResults: 200,
    },
    threshold: 2,
  });

  postCodeInput.addEventListener("selection", function (event) {
    postCodeAutocomplete.input.blur();
    postCodeAutocomplete.input.value = event.detail.selection.value;
    event.target.closest("form").submit();
  });

  document.querySelectorAll("[data-postcode]").forEach(link => {
    link.addEventListener("click", function (event) {
      event.preventDefault();

      postCodeAutocomplete.input.value = event.target.dataset.postcode;
      event.target.closest("form").submit();
    });
  });
}

const locationAddressInput = document.querySelector("#LocationAddress");
const latitudeInput = document.querySelector("#latitude");
const longitudeInput = document.querySelector("#longitude");
if (locationAddressInput && latitudeInput && longitudeInput) {
  const addressAutocomplete = new autoComplete({
    selector: "#LocationAddress",
    data: {
      src: async (query) => {
        try {
          const source = await fetch(`/api/addresses?query=${query}`);
          return await source.json();
        } catch (error) {
          return error;
        }
      },
      keys: ["name"]
    },
    debounce: 500,
    resultItem: {
      highlight: true,
    },
    resultsList: {
      maxResults: 200,
    },
    threshold: 5,
  });

  locationAddressInput.addEventListener("selection", function (event) {
    addressAutocomplete.input.blur();
    addressAutocomplete.input.value = event.detail.selection.value.name;
    latitudeInput.value = event.detail.selection.value.latitude;
    longitudeInput.value = event.detail.selection.value.longitude;
    latitudeInput.dispatchEvent(new Event("input"));
  });
}
