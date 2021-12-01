var rturgd = {
    TitleNew : '',
    TitleDel : '',
    TitleRen : '',
    TitleOK : '',
    TitleCancel : '',
    TitleClose: '',
    FormNewId: '',
    FormDelId: '',
    FormRenId:''
};

function openEditDialog(url, titleText, okButtonText, cancelButtonText, okButtonFormSelector) {
    var thisfn = 'RolesToUserRoleGroupsDialog.js openEditDialog function: ';
    console.log(thisfn + 'PING url, titleText, okButtonText, cancelButtonText, okButtonFormSelector', url, titleText, okButtonText, cancelButtonText, okButtonFormSelector);
    var okButtonStyle = '  bootboxAction-btn-ok';
    if (okButtonFormSelector === rturgd.FormNewId) {
        okButtonStyle = 'btn btn-success' + okButtonStyle;
    } else if (okButtonFormSelector === rturgd.FormDelId) {
        okButtonStyle = 'btn btn-danger' + okButtonStyle;
    } else {
        okButtonStyle = 'btn btn-primary' + okButtonStyle;
    }
    $.ajax({
        cache: false,
        url: url,
        type: 'get',
        contenttype: 'application/json',
        datatype: 'json',
        data: null,
        success: function (responseData) {
            if (!!responseData[0] && responseData[0].ErrorMessage) {
                bootbox.alert(responseData[0].ErrorMessage);
            } else {
                buttons = {
                    ok: {
                        label: okButtonText,
                        className: okButtonStyle,
                        callback: function () {
                            $('#' + okButtonFormSelector).submit();
                            return false;
                        }
                    },
                    cancel: {
                        label: cancelButtonText,
                        className: 'btn btn-secondary bootboxAction-btn-cancel',
                        callback: function () {
                            console.log(thisfn + 'cancel');
                            dialog.modal('hide');
                            location.reload();
                        }
                    }
                };
                var dialog = bootbox.dialog({
                    show: false,
                    title: titleText,
                    message: responseData,
                    size: 'small',
                    onEscape: function (event) {
                        console.log(thisfn + 'onEscape');
                        dialog.modal('hide');
                        location.reload();
                    },
                    buttons: buttons
                });
                dialog.modal('show');
            }
        },
        error: function (jqXHR, exception) {
            console.log(thisfn + 'Ajax hívás sikertelen! ', jqXHR.responseText);
        }
    });
}