var imageCropper;
var imageCanvas = document.getElementById("eventImageCanvas");
var imageCanvasContext = imageCanvas.getContext('2d');
var uploadedWidth;
var uploadedHeight;

var eventImageCropLeft = document.getElementById("eventImageCropLeft");
var eventImageCropTop = document.getElementById("eventImageCropTop");
var eventImageCropWidth = document.getElementById("eventImageCropWidth");
var previousImage = document.getElementById("previousImage");

window.onload = function () {
  imageCanvas.width = imageCanvas.clientWidth;
  imageCanvas.height = imageCanvas.clientHeight;

  var defaultImage = new Image();
  if (previousImage?.value)
    defaultImage.src = "/" + previousImage.value;
  else
    defaultImage.src = "/img/placeholder.svg";

  defaultImage.onload = function () {
    imageCanvasContext.drawImage(defaultImage, 0, 0, imageCanvas.width, imageCanvas.height);
  };

  window.addEventListener('mouseup', updatePreview);
  window.addEventListener('touchend', updatePreview);
};

function updatePreview() {
  if (imageCropper && imageCropper.isImageSet()) {
    var bounds = imageCropper.getCropBounds();
    var width = bounds.right - bounds.left;
    eventImageCropWidth.value = width;
    eventImageCropLeft.value = bounds.left;
    eventImageCropTop.value = uploadedHeight - bounds.top;
  }
}

function handleFileSelect(evt) {

  imageCropper = new ImageCropper(imageCanvas, 0, 0, imageCanvas.width, imageCanvas.height, true);

  var file = evt.target.files[0];
  var reader = new FileReader();
  var uploadedImage = new Image();

  uploadedImage.addEventListener("load", function () {
    imageCropper.setImage(uploadedImage);
    uploadedWidth = uploadedImage.width;
    uploadedHeight = uploadedImage.height;
    updatePreview();
  }, false);

  reader.onload = function () {
    uploadedImage.src = reader.result;
  };

  if (file) {
    reader.readAsDataURL(file);
  }
}

document.getElementById('eventImage').addEventListener('change', handleFileSelect, false);
