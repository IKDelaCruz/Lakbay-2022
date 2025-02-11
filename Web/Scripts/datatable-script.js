var PARAMS = PARAMS || (function () {
    return {
        init: function (
            tableElementId,
            emptyTableString,
            requestUrl,
            requestData,
            Template,
            processing,
            tdClassname,
            dom,
            searching
        ) {
            $.fn.dataTable.ext.errMode = 'throw';
            table = $(tableElementId).DataTable({
                "ordering": false,
                "info": false,
                "searching": searching,
                'processing': true,
                'language': {
                    'loadingRecords': '&nbsp;',
                    'processing': processing,
                    'emptyTable': emptyTableString
                },
                "dom": dom,
                "bLengthChange": false,
                "pageLength": 5,
                "ajax": {
                    "url": requestUrl,
                    "type": "GET",
                    "datatype": "JSON",
                    "data": requestData,
                    "dataSrc": function (json) {
                        rowDatas = json.data;
                        return rowDatas;
                    },
                    complete: function () {

                        //if (focusOnFirstReview) {
                        //    focusOnFirstReview = false;

                        //    //$(tableElementId).find('tbody').find('tr:first').focus();
                        //    $('html, body').animate({scrollTop: $(tableElementId).offset().top - 200}, 200);
                        //}

                    }
                },
                "initComplete": function () {

                },

                "columns": [
                    {
                        className: tdClassname,
                        render: function (data, type, full, meta) {
                            return Template(full);
                        }
                    }
                ]
            });
            table.on('page.dt', function () {
                $('html, body').animate({
                    scrollTop: $(".dataTables_wrapper").offset().top
                }, 'slow');
            });
        }
    };
}());

function RefreshView() {
    table.ajax.reload(null, false);
}

