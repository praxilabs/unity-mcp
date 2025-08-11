mergeInto(LibraryManager.library, {
    ConvertTableToExcel: function(jsonDataPtr, fileNamePtr) {
        var jsonData = UTF8ToString(jsonDataPtr);
        var fileName = UTF8ToString(fileNamePtr);

        if (typeof ExcelJS === 'undefined') {
            var script = document.createElement('script');
            script.src = "https://cdn.jsdelivr.net/npm/exceljs/dist/exceljs.min.js";
            script.onload = function() {
                processExcel(jsonData, fileName);
            };
            script.onerror = function() {
                console.error("Error loading ExcelJS.");
            };
            document.head.appendChild(script);
        } else {
            processExcel(jsonData, fileName);
        }

        function processExcel(data, fileName) {
            var workbook = new ExcelJS.Workbook();
            var parsedData = JSON.parse(data);

            parsedData.sheets.forEach(sheetData => {
                var worksheet = workbook.addWorksheet(sheetData.name);

                // Add logo if exists
                if (sheetData.logoBase64) {
                    var imageId = workbook.addImage({
                        base64: sheetData.logoBase64,
                        extension: 'png',
                    });
                    worksheet.addImage(imageId, 'A1:B4'); // Adjust as needed
                }

                // Add description row (with rich text support)
                var descriptionRow = worksheet.addRow([convertToRichText(sheetData.description)]);
                worksheet.addRow([]); // Add empty row

                // Add headers row
                var headerRow = worksheet.addRow(sheetData.headers);
                headerRow.font = { bold: true };

                // Add rows of data
                sheetData.rows.forEach(rowData => {
                    var row = worksheet.addRow(rowData.row.map(cell => convertToRichText(cell)));
                });
            });

            workbook.xlsx.writeBuffer().then(buffer => {
                var blob = new Blob([buffer], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                var url = URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.href = url;
                a.download = fileName + ".xlsx";
                a.style.display = 'none';
                document.body.appendChild(a);
                a.click();
                URL.revokeObjectURL(url);
                document.body.removeChild(a);
            });
        }

        // Recursive function to convert text with rich text tags to ExcelJS format
        function convertToRichText(text) {
            if (typeof text !== 'string') return text;

            function processText(text) {
                const tagRegex = /<(\w+?)(?:=(#?[a-zA-Z0-9]+))?>(.*?)<\/\1>/g;
                let resultArray = [];
                let lastIndex = 0;

                let match;
                while ((match = tagRegex.exec(text)) !== null) {
                    const fullMatch = match[0];
                    const tag = match[1];
                    const attribute = match[2];
                    const content = match[3] || ''; // Ensure content is a string

                    const offset = match.index;

                    // Add any plain text before this tag
                    if (offset > lastIndex) {
                        resultArray.push({ text: text.substring(lastIndex, offset) });
                    }

                    // Process the inner content recursively
                    let innerText = processText(content);

                    // Apply the style for the tag
                    let style = {};
                    switch (tag) {
                        case 'b':
                            style.bold = true;
                            break;
                        case 'i':
                            style.italic = true;
                            break;
                        case 'u':
                            style.underline = true;
                            break;
                        case 'strikeThrough':
                            style.strike = true;
                            break;
                        case 'sup':
                            style.vertAlign = 'superscript';
                            break;
                        case 'sub':
                            style.vertAlign = 'subscript';
                            break;
                        case 'color':
                            if (attribute) {
                                style.color = { argb: convertHexToArgb(attribute) };
                            }
                            break;
                        case 'size':
                            if (attribute) {
                                style.size = parseInt(attribute);
                            }
                            break;
                        default:
                            break;
                    }

                    // Apply the style to the inner content
                    innerText.forEach(function(item) {
                        var mergedFont = Object.assign({}, item.font, style);
                        resultArray.push({
                            text: item.text,
                            font: mergedFont
                        });
                    });

                    lastIndex = tagRegex.lastIndex;
                }

                // Add any remaining plain text after the last tag
                if (lastIndex < text.length) {
                    resultArray.push({ text: text.substring(lastIndex) });
                }

                return resultArray;
            }

            const processedText = processText(text);
            return { richText: processedText };
        }

        function convertHexToArgb(hex) {
            hex = hex.replace(/^#/, '');
            if (hex.length === 3) {
                hex = hex.split('').map(char => char + char).join('');
            }
            return 'FF' + hex.toUpperCase();
        }
    }
});
