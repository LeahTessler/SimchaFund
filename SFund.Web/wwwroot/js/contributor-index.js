
$(() => {

    const modal = new bootstrap.Modal($("#show-modal")[0]);
    const modaldeposit = new bootstrap.Modal($(".deposit")[0]);
   /* const editmodal = new bootstrap.Modal($(".edit")[0]);*/
    $("#new-contributor").on('click', function () {
        modal.show();
    })

    

    $(".deposit-button").on('click', function () {
        const contributorId = $(this).data('contributorid');
        $("#hidden-contribid").val(contributorId);

        const name = $(this).data('contributorname');
        $("#deposit-name").text(name);
       
        modaldeposit.show();
       
    })

    $(".edit-contrib").on('click', function () {
        const id = $(this).data('id');
        const firstName = $(this).data('firstName');
        const lastName = $(this).data('lastName');
        const cell = $(this).data('cell');
        const alwaysInclude = $(this).data('alwaysInclude');
        const date = $(this).data('date');
        const form = $(".new-contrib form");
        $(".new-contrib h5").text('Edit Contributor');
        $("#edit-id").remove();
        $(".new-contrib form").append(`<input type="hidden" id="edit-id" name="id" value=${id}>`)
        $("#initialDepositDiv").hide();
        $("#contributor_first_name").val(firstName);
        $("#contributor_last_name").val(lastName);
        $("#contributor_cell_number").val(cell);
        $("#contributor_always_include").prop('checked', alwaysInclude === "True");
        $("#contributor_created_at").val(date);
       
        form.attr('action', '/contributors/update');
        modal.show();
    });


  
   
   
});