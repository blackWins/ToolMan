$(function () {
    require.config({ paths: { vs: '../libs/monaco-editor/min/vs' } });
    require(['vs/editor/editor.main'], function () { });

    var l = abp.localization.getResource('ToolMan');
    var templateService = toolMan.services.template;

    let currentPath = '';

    $('#GenerateLButton').click(function (e) {
        Generate($("#ViewModel_TemplatePath").val());
    })

    $('#GenerateRButton').click(function (e) {
        Generate(currentPath);
    })

    function Generate(path) {
        var input = {
            templatePath: path,
            outputPath: $("#ViewModel_OutputPath").val(),
            options: $("#ViewModel_Options").val()
        }
        if (input.templatePath == '') {
            abp.notify.error(l('TemplatePath Or TemplateFile Is Empty'));
            return
        }
        if (input.outputPath == '') {
            abp.notify.error(l('OutputPath Is Empty'));
            return
        }

        toolMan.services.genericGenerate.run(input).done((res) => {
            abp.notify.info(l('Generate Succesful'))
        })
    }

    $("#UpdateBtn").click(function (e) {
        templateService.updateContent({ path: currentPath, content: $("#Template").val() }).done((res) => {
            abp.notify.info(l('Update Succesful'))
            Preview();
        })
    })

    $("#ViewModel_Options").on('change', Preview)

    function Preview() {
        if (currentPath == '') return;
        var input = {
            path: currentPath,
            options: $("#ViewModel_Options").val()
        }

        $('#Preview').removeAttr('data-highlighted class');

        templateService.preview(input)
            .done((res) => {
                var arr = currentPath.split(/\\/);
                var fileName = '/**** ' + arr[arr.length - 1] + ' ****/\r\n\r\n';
                $('#Preview').text(fileName + res);
                hljs.highlightElement($("#Preview")[0]);
                $("#Preview_modal").html($("#Preview").html());
            })
    }

    /************************************************************************
     * jstree Configurations                                                *
     ************************************************************************/

    $("#OpenAllBtn").click(function (e) {
        $('#jstree').jstree('open_all');
    })
    $("#CloseAllBtn").click(function (e) {
        $('#jstree').jstree('close_all');
    })
    $("#RefreshBtn").click(function (e) {
        LoadTree();
    })
    $("#ViewModel_TemplatePath").on('input', function (e) {
        LoadTree();
    })

    function LoadTree() {
        var path = $("#ViewModel_TemplatePath").val();
        if (path == '' || path.indexOf(':') == -1) return;
        templateService.getDirectoryTree(path)
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
                    items.ccp = {
                        label: l(items.ccp.label),
                        action: (data) => {
                            //var refTree = $.jstree.reference(data.reference);
                            //var filePath = refTree.get_node(data.reference).original.filePath;
                            if (node.icon !== 'jstree-folder') {
                                var name = currentPath.split(/\\/).pop();
                                CreateTab(name);
                            }
                        }
                    };

                    items.create.label = l(items.create.label)
                    items.rename.label = l(items.rename.label)
                    items.remove.label = l(items.remove.label)
                    if (arr.length > 1 && arr[arr.length - 1].indexOf('}}') == -1) {
                        delete items.create;
                    }
                    else {
                        delete items.ccp;
                    }
                    return items
                }
            },
        }).on('select_node.jstree', (event, record) => {
            currentPath = record.node.original.filePath;
            Preview();
        }).on('delete_node.jstree', function (event, record) {
            if (!record.node.original.filePath) {
                record.instance.refresh();
                return
            }
            abp.message.confirm(l('Confirm the permanent deletion {0}', record.node.text))
                .then(function (confirmed) {
                    if (confirmed && record.node.original.filePath) {
                        templateService.delete(record.node.original.filePath)
                            .then(function () {
                                abp.notify.info(l('SuccessfullyDeleted'));
                            });
                    }
                    else {
                        record.instance.refresh();
                    }
                });
        }).on('rename_node.jstree', function (event, record) {
            if (record.old == record.text) return
            var filePath = record.node.original.filePath;
            if (!filePath) {
                var p_record = $('#jstree').jstree('get_node', record.node.parent)
                var path = p_record.original.filePath + '\\' + record.text;
                templateService.create(path).done((res) => { GetTemplate(); })
                //set node filePath
                var originalData = record.node.original;
                originalData.filePath = path;
                record.node.original = originalData;
                return
            }
            templateService.rename(filePath, record.text).done(() => {
                var originalData = record.node.original;
                originalData.filePath = filePath.replace(record.old, record.text);
                record.node.original = originalData;
            }).catch(() => { record.instance.refresh() });
        }).on('move_node.jstree', function (event, record) {
            var p_record = $('#jstree').jstree('get_node', record.parent)
            templateService.move(record.node.original.filePath, p_record.original.filePath).done((res) => {
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

    /************************************************************************
     * JSONEditor Configurations                                            *
     ************************************************************************/
    const jsonEditorOptions = {
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
    const jsonEditor = new JSONEditor(document.getElementById("jsoneditor"), jsonEditorOptions)

    /*
    
    jsonEditor.options.templates = [{
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
    
    jsonEditor.set(initialJson);
    
    */

    /************************************************************************
     * DynamicTab Configurations                                            *
     ************************************************************************/
    function CreateTab(name) {
        var id = "Tab" + name.replace(/[{}.@]/g, '_');
        var tabHeader = id + "-tab";

        if ($("#" + tabHeader).length > 0) {
            $("#" + tabHeader)[0].click();
            return;
        }

        var tabBtn = abp.utils.formatString('<li class="dynamic-tab nav-item" role="presentation"><a aria-controls="{0}" aria-selected="false" class="nav-link" data-bs-toggle="tab" href="#{0}" id="{0}-tab" role="tab" title="{2}">{1} <i class="fa fa-close" title="{3}" data-id="{0}"></i></a></li>', id, name, currentPath, l("Close"));

        var tabContent = abp.utils.formatString('<div aria-labelledby="{0}-tab" class="dynamic-tab-content tab-pane fade" id="{0}" role="tabpanel"></div>', id, currentPath);

        $("#Tabs").append(tabBtn);
        $("#TabsContent").append(tabContent);

        CreateMonacoEditor(id, name.split('.').pop());

        $("#" + tabHeader)[0].click();
    }

    $(document).on('click', '.dynamic-tab > a .fa-close', function () {
        var tab = $(this).parents('li');
        var id = $(this).attr('data-id');
        var currentTabId = tab.find('a').attr('href');
        var filePath = tab.find('a').attr('title');
        var preTab = tab.prev('li').find('a').attr('id');
        var nextTab = tab.next('li').find('a').attr('id');

        var contentDispose = (content) => {
            content.dispose();
            tab.remove();
            $(currentTabId).remove();
            nextTab ? $("#" + nextTab)[0].click() : $("#" + preTab)[0].click();
            if (modelChangeList.indexOf(id) != -1) {
                modelChangeList.splice(modelChangeList.indexOf(id), 1)
            }
        }

        document.querySelectorAll('.dynamic-tab > a').forEach((element, index) => {
            if (element.getAttribute('href') == currentTabId) {
                var content = monaco.editor.getModels()[index];
                if (modelChangeList.indexOf(id) == -1) {
                    contentDispose(content);
                } else {
                    SaveChangeConfirm(l('MonacoEditorSaveChangeConfirmationMessage'))
                        .then(function (confirmed) {
                            if (confirmed === undefined) {
                                return;
                            }
                            if (confirmed) {
                                templateService.updateContent({ path: filePath, content: content.getValue() }).done((res) => {
                                    contentDispose(content);
                                }).fail(() => {
                                    abp.notify.error(l('Save Failed'));
                                })
                            } else if (confirmed === false) {
                                contentDispose(content);
                            }
                        });
                }
                return;
            }
        });
    });

    SaveChangeConfirm = function (title, callback) {

        var userOpts = {
            title: title,
            showCancelButton: false,
            showDenyButton: true
        };

        var opts = $.extend(
            {},
            abp.libs.sweetAlert.config['default'],
            abp.libs.sweetAlert.config.confirm,
            userOpts
        );

        return $.Deferred(function ($dfd) {
            Swal.fire(opts).then(result => {
                callback && callback(result.value);
                $dfd.resolve(result.value);
            })
        });
    };

    /************************************************************************
     * MonacoEditor Configurations                                          *
     ************************************************************************/
    let modelChangeList = [];
    function CreateMonacoEditor(id, fileExtension) {
        let languages = monaco.languages.getLanguages();
        let language = languages.find(lang => lang.extensions?.includes('.' + fileExtension));

        templateService.getContent(currentPath).done((res) => {
            let model = monaco.editor.create(document.getElementById(id), {
                acceptSuggestionOnCommitCharacter: true,
                acceptSuggestionOnEnter: 'on',
                accessibilitySupport: 'on',
                autoClosingBrackets: 'always',
                autoClosingDelete: 'always',
                autoClosingOvertype: 'always',
                autoClosingQuotes: 'always',
                autoIndent: 'None',
                automaticLayout: true,
                codeLens: false,
                value: res,
                theme: 'vs-ligth',
                fontSize: 14,
                language: language?.id ?? 'liquid'
            });

            model.onDidChangeModelContent(function () {
                if (modelChangeList.indexOf(id) == -1) {
                    modelChangeList.push(id);
                }
            });

            model.onKeyDown((e) => {
                if (e.keyCode === 49 /** KeyCode.KeyS */ && e.ctrlKey) {
                    var path = $("#" + id + "-tab").attr("title")
                    templateService.updateContent({ path: path, content: model.getValue() }).done((res) => {
                        abp.notify.success(l('Successfully Saved'))
                        if (modelChangeList.indexOf(id) != -1) {
                            modelChangeList.splice(modelChangeList.indexOf(id), 1)
                        }
                    }).fail(() => {
                        abp.notify.error(l('Save Failed'));
                    })
                    e.preventDefault();
                }
            });
        })
    }
});
