const confirmarPendientes = document.getElementById('confirmar-pendientes');

confirmarPendientes.addEventListener('click', () => {
    var badge = confirmarPendientes.querySelector('span.badge')
    if (!confirmarPendientes.dataset.nuevo) {
        $.post('/Home/ConfirmarAlertas', {}, function () {
            if (badge) confirmarPendientes.removeChild(badge)
            confirmarPendientes.dataset.nuevo = 'true'
        });
    }
})