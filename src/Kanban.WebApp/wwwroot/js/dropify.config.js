$('.dropify').dropify({
    messages: {
        'default': 'Arrastrar y soltar un archivo aqui o click',
        'replace': 'Arrastrar y soltar un archivo aqui o click para reemplazar',
        'remove': 'Eliminar',
        'error': 'Ooops, occurrio un error.'
    },

    error: {
        'fileSize': 'The file size is too big ({{ value }} max).',
        'minWidth': 'The image width is too small ({{ value }}}px min).',
        'maxWidth': 'The image width is too big ({{ value }}}px max).',
        'minHeight': 'The image height is too small ({{ value }}}px min).',
        'maxHeight': 'The image height is too big ({{ value }}px max).',
        'imageFormat': 'The image format is not allowed ({{ value }} only).'
    },
    //tpl: {
    //    wrap: '<div class="dropify-wrapper"></div>',
    //    loader: '<div class="dropify-loader"></div>',
    //    message: '<div class="dropify-message"><span class="file-icon" /> <p>{{ default }}</p></div>',
    //    preview: '<div class="dropify-preview"><span class="dropify-render"></span><div class="dropify-infos"><div class="dropify-infos-inner"><p class="dropify-infos-message">{{ replace }}</p></div></div></div>',
    //    filename: '<p class="dropify-filename"><span class="file-icon"></span> <span class="dropify-filename-inner"></span></p>',
    //    clearButton: '<button type="button" class="dropify-clear">{{ remove }}</button>',
    //    errorLine: '<p class="dropify-error">{{ error }}</p>',
    //    errorsContainer: '<div class="dropify-errors-container"><ul></ul></div>'
    //}
});