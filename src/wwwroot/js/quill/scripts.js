const quill = new Quill('#editor', {
  theme: 'snow'
});

quill.on('text-change', () => {
  document.querySelector('#editorValue').value = quill.getSemanticHTML();
});
