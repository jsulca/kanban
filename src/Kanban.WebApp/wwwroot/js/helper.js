var helper = {
    errorShow: function (id, message) {
        var html = `<div class="alert alert-danger alert-dismissible" role="alert">
                        <strong><i class="fa fa-exclamation-circle"></i> Error: </strong> 
                        <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        ${message}
                    </div>`;

        $(id).hide().html(html).slideDown(1000);
    },
    wait: {
        modal: function (idModal) {
            var htmlText = '<div class="modal-dialog">'
                + '<div class="modal-content modal-width">'
                + '<div class="modal-header">'
                + '<h4 class="modal-title">Cargando</h4>'
                + '</div>'
                + '<div class="modal-body form-horizontal">'
                + '<div id="uploadProgress" class="progress progress-striped active">'
                + '<div class="progress-bar" role="progressbar" aria-valuenow="10" aria-valuemin="0" aria-valuemax="100" style="width: 100%">'
                + '<span class="sr-only">Cargando</span>'
                + '</div>'
                + '</div>'
                + '</div>'
                + '</div>'
                + '</div>';
            $("#" + idModal).html(htmlText);
        },
        append: function (selector) {
            var htmlText = '<div class="progress progress-striped active helper-progressbar ">'
                + '<div class="progress-bar" role="progressbar" aria-valuenow="10" aria-valuemin="0" aria-valuemax="100" style="width: 100%">'
                + '<span class="sr-only">Cargando</span>'
                + '</div>'
                + '</div>';

            var a = selector.charAt(0);
            if (a != '.' && a != '#') selector = '#' + selector;

            $(selector).prepend(htmlText);
        },
        remove: function (selector) {
            var a = selector.charAt(0);
            if (a != '.' && a != '#') selector = '#' + selector;

            $(selector + ' .helper-progressbar').remove();
        }
    },
    pagination: {
        btnNames: ['<i class="fa fa-angle-left"></i><i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right">', '<i class="fa fa-angle-right"></i><i class="fa fa-angle-right"></i>'],
        btnPages: 7,
        btnClass: 'pagination pagination-sm mb-0 mt-0',
        update: function (table, btnPages, btnNames, btnClass) {
            if (btnPages == null) btnPages = helper.pagination.btnPages;
            if (btnNames == null) btnNames = helper.pagination.btnNames;
            if (btnClass == null) btnClass = helper.pagination.btnClass;

            var paginator = $(table.data('paginator'));
            var totaltext = $(table.data('totaltext'));
            var page = table.data('pageindex');
            var fn = table.data('function');
            var nPaginas = Math.ceil(table.data('totalrows') / table.data('pagesize'));
            paginator.html(null);

            var ul = $('<ul>');
            ul.addClass(btnClass);

            var li = $('<li>', { class: 'page-item' });
            li.addClass(page == 1 ? 'disabled' : '');
            li.append($('<a>', { class: "page-link" }));
            li.children('a').html(btnNames[0]);
            li.children('a').attr('href', page == 1 ? 'javascript:void(0)' : `javascript:${fn}(1)`);
            ul.append(li);

            li = $('<li>', { class: 'page-item' });
            li.addClass(page == 1 ? 'disabled' : '');
            li.append($('<a>', { class: "page-link" }));
            li.children('a').html(btnNames[1]);
            li.children('a').attr('href', page == 1 ? 'javascript:void(0)' : `javascript:${fn}(${page - 1})`);
            ul.append(li);

            if (nPaginas > 1) {
                var g = page;
                if (page < Math.ceil(btnPages / 2)) g = Math.ceil(btnPages / 2);
                if (page > nPaginas - Math.ceil(btnPages / 2) + 1) g = nPaginas - Math.ceil(btnPages / 2) + 1;

                var ini = g - Math.ceil(btnPages / 2) + 1 <= 0 ? 1 : g - Math.ceil(btnPages / 2) + 1;
                var fin = nPaginas <= btnPages ? nPaginas : g - Math.ceil(btnPages / 2) + btnPages;

                for (var i = ini; i <= fin; i++) {
                    li = $('<li>', { class: 'page-item' });
                    li.addClass(page == i ? 'active' : '');
                    li.append($('<a>', { class: "page-link" }));
                    li.children('a').html(i);
                    li.children('a').attr('href', page == i ? 'javascript:void(0)' : `javascript:${fn}(${i})`);
                    ul.append(li);
                }
            }

            li = $('<li>', { class: 'page-item' });
            li.addClass(page == nPaginas || nPaginas == 0 ? 'disabled' : '');
            li.append($('<a>', { class: "page-link" }));
            li.children('a').html(btnNames[2]);
            li.children('a').attr('href', page == nPaginas || nPaginas == 0 ? 'javascript:void(0)' : `javascript:${fn}(${page + 1})`);
            ul.append(li);

            li = $('<li>', { class: 'page-item' });
            li.addClass(page == nPaginas || nPaginas == 0 ? 'disabled' : '');
            li.append($('<a>', { class: "page-link" }));
            li.children('a').html(btnNames[3]);
            li.children('a').attr('href', page == nPaginas || nPaginas == 0 ? 'javascript:void(0)' : `javascript:${fn}(${nPaginas})`);
            ul.append(li);

            paginator.append(ul);
            totaltext.text('(' + page + ' de ' + nPaginas + ') - ' + table.data('totalrows') + ' registros');
        }
    },
    format: {
        decimal: function (value) {
            return Number(value).toFixed(1).replace(/(\d)(?=(\d{3})+\.)/g, '$1');
        },
        decimalEspecial: function (value, n) {
            var re = '\\d(?=(\\d{3})+' + (n > 0 ? '\\.' : '$') + ')';
            return Number(value).toFixed(Math.max(0, ~ ~n)).replace(new RegExp(re, 'g'), '$&,');
        }
    }
};

setInterval(function () {
    $.get('/Home/Stay', function (data) { });
}, (2.5 * 60) * 1000);