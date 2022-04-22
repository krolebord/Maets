// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$('input[data-provide="datepicker"]').datepicker();

$('select.search-select').select2({
    theme: "bootstrap-5",
    width: '100%'
});

$('select[multiple=true].search-select').select2({
    theme: "bootstrap-5",
    width: '100%',
    closeOnSelect: false
});

$('select.labels-select').select2({
    theme: "bootstrap-5",
    width: '100%',
    tags: true,
    closeOnSelect: false,
    tokenSeparators: [',']
});

$('table.table-net').DataTable( {
    paging: false,
    ordering: false,
    info: false
} );
