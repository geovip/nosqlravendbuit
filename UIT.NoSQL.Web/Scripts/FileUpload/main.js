/*
 * jQuery File Upload Plugin JS Example 7.0
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint nomen: true, unparam: true, regexp: true */
/*global $, window, document */

$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
		
        url: '/Files',
        //acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        maxFileSize: 3000000,
    }).bind('fileuploadstopped', function (e, data) {
        $('.template-download').each(function (index, domEle) {
            var nameElement = domEle.cells[0].innerHTML;
            var sizeElement = '(' + domEle.cells[1].innerHTML + ')';
            var deleteElement = $('<a href="#" class="delete-file" style="margin-left:10px" data-url="' + $(domEle.cells[3].children[0]).attr("data-url") + '">Delete</a>');
            var tmpl = $('<div class="file"></div>').append(nameElement).append(sizeElement).append(deleteElement);
            
            tmpl.appendTo(currentPositionAttachFile.prev().prev()); // lay div class="files"

        });

        $('.box-attach-files').css('visibility', 'hidden');
        $('.template-download').remove();
        $('.template-upload').remove();
    });

    // Enable iframe cross-domain access via redirect option:
    $('#fileupload').fileupload(
        'option',
        'redirect',
        window.location.href.replace(
            /\/[^\/]*$/,
            '/cors/result.html?%s'
        )
    );

    // Load existing files:
    $.ajax({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        url: $('#fileupload').fileupload('option', 'url'),
        dataType: 'json',
        context: $('#fileupload')[0]
    }).done(function (result) {
        $(this).fileupload('option', 'done')
            .call(this, null, {result: result});
    });

});
