const autoCompleteJS = new autoComplete({
  selector: "#postCode",
  data: {
    src: async (query) => {
      try {
        const source = await fetch(`/api/postcodes?query=${query}`);
        const data = await source.json();

        return data;
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

document.querySelector("#postCode").addEventListener("selection", function (event) {
  autoCompleteJS.input.blur();

  const selection = event.detail.selection.value;
  autoCompleteJS.input.value = selection;

  event.target.closest("form").submit();
});

document.querySelectorAll("[data-postcode]").forEach(link => {
  link.addEventListener("click", function (event) {
    event.preventDefault();

    const selection = event.target.dataset.postcode;
    autoCompleteJS.input.value = selection;

    event.target.closest("form").submit();
  });
});
