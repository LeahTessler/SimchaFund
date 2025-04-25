
$(() => {
   
        const modal = new bootstrap.Modal($("#show-modal")[0]);
        $("#new-simcha").on('click', function () {
            modal.show();
        });
});