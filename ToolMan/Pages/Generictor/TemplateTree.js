
var l = abp.localization.getResource('BookStore');

var service = acme.bookStore.identityServer.organizationUnit;

//import jstree

var map = {
    id: 'id', parent: 'parentId', text: 'displayName'
}

$('#jstree').jstree({
    core: {
        check_callback: true,
        data: function (node, callback) {
            var map = this.settings.map;
            service.getChildren({ id: node.id === '#' ? '' : node.id }).done((res) => {
                if (map) {
                    res.map(x => {
                        x.parent = x[map.parent] || '#';
                        x.text = x[map.text];
                        x.id = x[map.id];
                        x.children = true;
                        return x;
                    });
                }
                callback(res)
            })
        }
    },
    map,
    contextmenu: {
        items: function (node) {
            var items = $.jstree.defaults.contextmenu.items();
            delete items.ccp;
            items.create.text = l(items.create.text)
            items.rename.text = l(items.rename.text)
            items.remove.text = l(items.remove.text)
            if (!abp.auth.isGranted('acme.BookStore.OrganizationUnit.Create')) delete items.create
            if (!abp.auth.isGranted('acme.BookStore.OrganizationUnit.Update')) delete items.rename
            if (!abp.auth.isGranted('acme.BookStore.OrganizationUnit.Delete')) delete items.remove
            return items
        }
    },
    plugins: ['wholerow', 'state', 'contextmenu', 'dnd']
}).on('delete_node.jstree', function (e, data) {
    abp.message.confirm(l('DeletionConfirmationMessage', data.node.text))
        .then(function (confirmed) {
            if (confirmed) {
                service.delete(data.node.id)
                    .then(function () {
                        abp.notify.info(l('SuccessfullyDeleted'));
                        data.instance.refresh();
                    });
            }
        });
    data.instance.refresh();
}).on('create_node.jstree', function (p, n) {
    var input = {};
    input[map['parent']] = n.parent;
    input[map['text']] = n.node.text;
    service.create(input)
        .done((res) => { n.instance.set_id(n.node, res.id) })
        .catch(() => { n.instance.refresh() });
}).on('rename_node.jstree', function (p, n) {
    if (n.old == n.text) return
    var input = {}; input[map['text']] = n.text;
    service.update(n.node.id, input)
        .catch(() => { n.instance.refresh() });
}).on('move_node.jstree', function (p, n) {
    var input = { id: n.node.id };
    input[map['parent']] = n.node.parent;
    service.move(input)
        .then(() => { n.instance.refresh() })
        .catch(() => { n.instance.refresh() });
}).on('changed.jstree', function (p, n) {
});