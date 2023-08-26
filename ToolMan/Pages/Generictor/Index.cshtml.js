$(function () {

    var getData = function () {
        var input = {};
        $("#ViewModel").serializeArray().forEach(function (data) {
            input[data.name.replace(/ViewModel./g, '')] = data.value;
        })
        return input;
    };

    $('#GenerateButton').click(function (e) {
        toolMan.services.genericGenerate.run(getData()).done((res) => {
            abp.notify.info('Generate Succesful')
        })
    })

    /*
    // create the editor
    const container = document.getElementById("jsoneditor")
    const options = {}
    const editor = new JSONEditor(container, options)

    // set json
    const initialJson = {
        "Array": [1, 2, 3],
        "Boolean": true,
        "Null": null,
        "Number": 123,
        "Object": { "a": "b", "c": "d" },
        "String": "Hello World"
    }
    editor.set(initialJson)

    // get json
    const updatedJson = editor.get()

    var input = document.getElementById("ViewModel_Options");

    function formatJson() {
        try {
            var json = JSON.parse(input.value);
            var formattedJson = JSON.stringify(json, null, 4);
            input.value = formattedJson;
        } catch (e) {
            console.log("Error: Invalid JSON data!");
        }
    }

    //input.addEventListener("focus", formatJson);

    input.addEventListener("blur", formatJson);

    let fnKey = ['Tab', 'CapsLock', 'Shift', 'Control', 'Enter', 'Backspace', 'Escape'];
    let ignoreSymbol = ['[', '] ', '{', '}', '', ',']
    let symbol = ['', ' ', '{', ',']
    let symbolGroup = ['[', '{', '"']
    let _symbolGroup = [']', '}', '"']

    input.addEventListener("keydown", function (event) {
        console.log(event.key)
        var cursorPosition = input.selectionStart;
        var text = input.value;
        if (symbolGroup.indexOf(event.key) >= 0 && text.length > 0) {
            var i = symbolGroup.indexOf(event.key)
            input.setRangeText(_symbolGroup[i], cursorPosition, cursorPosition, "start");
        }
        console.log(text[cursorPosition])
        if (event.key === "Enter") {
            if (symbol.indexOf(text[cursorPosition - 1]) == -1) {
                input.setRangeText(',', cursorPosition, cursorPosition, "end");
            }
        }
        if (event.key === "Tab") {
            input.setRangeText("    ", cursorPosition, cursorPosition, "end");
            event.preventDefault();
        }

        var lineStart = text.slice(0, cursorPosition).lastIndexOf("\n") + 1;
        var lineEnd = text.indexOf("\n", cursorPosition);
        if (lineEnd === -1) {
            lineEnd = text.length;
        }
        var currentLine = text.slice(lineStart, lineEnd).trimStart();
        if (currentLine[0] !== '"' && ignoreSymbol.indexOf(currentLine) < 0) {
            input.setRangeText('"', cursorPosition - 1, cursorPosition - 1, "end");
            input.setRangeText('"', cursorPosition + 1, cursorPosition + 1, "start");
        }

        lineEnd -= text[lineEnd - 1] === ',' ? 1 : 0
        if (symbolGroup.indexOf(event.key) < 0 && fnKey.indexOf(event.key) < 0 && cursorPosition > 3 && text[lineEnd - 1] === ':' && text[lineEnd - 2] === '"') {
            input.setRangeText('"', cursorPosition, cursorPosition, "end");
            input.setRangeText('"', cursorPosition + 1, cursorPosition + 1, "start");
        }
    });
    */
});
