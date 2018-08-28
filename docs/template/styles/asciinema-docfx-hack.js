$(function () {
  renderASCIINema();

  function renderASCIINema() {
    // Replace img link to asciinema with embedded player
    jQuery('img.asciinema').each(function () {
      var $img = jQuery(this);
      var imgID = $img.attr('id');
      var imgClass = $img.attr('class');
      var imgURL = $img.attr('src');
      var rows = $img.attr('rows');
      var cols = $img.attr('cols');
      var loop = $img.attr('loop');
      var poster = $img.attr('poster');

      $img.replaceWith(`<asciinema-player src="${imgURL}" rows="${rows}" cols="${cols}" loop="${loop}" poster="${poster}" />`);
    });
  }
});
