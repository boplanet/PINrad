
document.addEventListener('DOMContentLoaded', function () {
    // Inicijalizacija Bootstrap tooltips (ako ih koristite)
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Primjer: Skripta za potvrdu prije brisanja
    var deleteButtons = document.querySelectorAll('.btn-delete');
    deleteButtons.forEach(function (button) {
        button.addEventListener('click', function (event) {
            if (!confirm('Jeste li sigurni da želite obrisati ovu stavku?')) {
                event.preventDefault();
            }
        });
    });
});
