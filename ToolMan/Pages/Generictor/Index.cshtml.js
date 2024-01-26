$(function () {

    var L = abp.localization.getResource('ToolMan');
    var Service = toolMan.services.template;

    let CurrentPath = '';

    $('#GenerateLButton').click(function (e) {
        Generate($("#ViewModel_TemplatePath").val());
    })

    $('#GenerateRButton').click(function (e) {
        Generate(CurrentPath);
    })

    function Generate(path) {
        var input = {
            templatePath: path,
            outputPath: $("#ViewModel_OutputPath").val(),
            options: $("#ViewModel_Options").val()
        }
        if (input.templatePath == '') {
            abp.notify.error(L('TemplatePath Or TemplateFile Is Empty'));
            return
        }
        if (input.outputPath == '') {
            abp.notify.error(L('OutputPath Is Empty'));
            return
        }

        toolMan.services.genericGenerate.run(input).done((res) => {
            abp.notify.info(L('Generate Succesful'))
        })
    }

    $("#UpdateBtn").click(function (e) {
        Service.updateContent({ path: CurrentPath, content: $("#Template").val() }).done((res) => {
            abp.notify.info(L('Update Succesful'))
            Preview();
        })
    })

    $("#ViewModel_Options").on('change', function (e) {
        Preview();
    })

    function HighLightCode() {
        hljs.highlightElement($("#Preview")[0]);
        $("#Preview_modal").html($("#Preview").html());
    }

    HighLightCode();

    function Preview() {
        if (CurrentPath == '') return;
        var input = {
            path: CurrentPath,
            options: $("#ViewModel_Options").val()
        }
        $('#Preview').removeAttr('data-highlighted class');
        Service.preview(input)
            .done((res) => {
                var arr = CurrentPath.split(/\\/);
                var fileName = '/**** ' + arr[arr.length - 1] + ' ****/\r\n\r\n';
                $('#Preview').text(fileName + res);
                HighLightCode()
            })
    }

    function GetTemplate() {
        if (CurrentPath == '') return;
        Service.getContent(CurrentPath)
            .done((res) => {
                $("#Template").val(res);
                Preview();
            })
    }

    $("#OpenAllBtn").click(function (e) {
        $('#jstree').jstree('open_all');
    })

    $("#CloseAllBtn").click(function (e) {
        $('#jstree').jstree('close_all');
    })

    $("#RefreshBtn").click(function (e) {
        LoadTree();
    });
    $("#ViewModel_TemplatePath").on('input', function (e) {
        LoadTree();
    });

    function LoadTree() {
        var path = $("#ViewModel_TemplatePath").val();
        if (path == '' || path.indexOf(':') == -1) return;
        Service.getDirectoryTree(path)
            .then((val) => {
                $('#jstree').jstree('destroy');
                RenderTree(val);
            })
    }

    function RenderTree(data) {
        $('#jstree').jstree({
            core: {
                check_callback: true,
                data: data,
                state: { 'core': { 'state': 'open' } }
            },
            plugins: [
                "contextmenu",
                "dnd",
                'wholerow',
                'state'
            ],
            contextmenu: {
                items: function (node) {
                    var arr = node.text.split('.');
                    var items = $.jstree.defaults.contextmenu.items();
                    delete items.ccp;
                    items.create.label = L(items.create.label)
                    items.rename.label = L(items.rename.label)
                    items.remove.label = L(items.remove.label)
                    if (arr.length > 1 && arr[arr.length - 1].indexOf('}}') == -1) {
                        delete items.create;
                    }
                    return items
                }
            },
        }).on('select_node.jstree', (event, record) => {
            CurrentPath = record.node.original.filePath;
            var arr = CurrentPath.split('.');
            if (arr.length > 1 && arr[arr.length - 1].indexOf('}}') == -1) {
                GetTemplate();
            }
        }).on('delete_node.jstree', function (event, record) {
            if (!record.node.original.filePath) {
                record.instance.refresh();
                return
            }
            abp.message.confirm(L('DeletionConfirmationMessage', record.node.text))
                .then(function (confirmed) {
                    if (confirmed && record.node.original.filePath) {
                        Service.delete(record.node.original.filePath)
                            .then(function () {
                                abp.notify.info(L('SuccessfullyDeleted'));
                            });
                    }
                    else {
                        record.instance.refresh();
                    }
                });
        }).on('rename_node.jstree', function (event, record) {
            if (record.old == record.text) return
            var filePath = record.node.original.filePath;
            debugger
            if (!filePath) {
                var p_record = $('#jstree').jstree('get_node', record.node.parent)
                var path = p_record.original.filePath + '\\' + record.text;
                Service.create(path).done((res) => { GetTemplate(); })
                //set node filePath
                var originalData = record.node.original;
                originalData.filePath = path;
                record.node.original = originalData;
                return
            }
            Service.rename(filePath, record.text).done(() => {
                var originalData = record.node.original;
                originalData.filePath = filePath.replace(record.old, record.text);
                record.node.original = originalData;
            }).catch(() => { record.instance.refresh() });
        }).on('move_node.jstree', function (event, record) {
            var p_record = $('#jstree').jstree('get_node', record.parent)
            Service.move(record.node.original.filePath, p_record.original.filePath).done((res) => {
                if (record.node.children.length == 0) {
                    var originalData = record.node.original;
                    originalData.filePath = res
                    record.node.original = originalData;
                    return
                }
                setTimeout(() => {
                    $("#ViewModel_TemplatePath").trigger("input");
                }, 500);
            }).catch(() => { record.instance.refresh() });
        });
    }
    
    const options = {
        search: false,
        modes: ['tree', 'code'],
        onChangeText: (v) => {
            try {
                var json = JSON.parse(v);
                if (json[''] === '') return;

                $("#ViewModel_Options").val(JSON.stringify(json));
                $("#ViewModel_Options").trigger("change")
            } catch (err) {
            }
        }
    }

    const editor = new JSONEditor(document.getElementById("jsoneditor"), options)

    /*
    
    editor.options.templates = [{
        text: 'Address',
        title: 'Insert a Address Node',
        field: 'AddressTemplate',
        value: {
            'street': "",
            'city': "",
            'state': "",
            'ZIP code': ""
        }
    }]
    
    editor.set(initialJson);
    
    */
});
