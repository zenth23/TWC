
$(function () {
    $('#dvDragFiles').on({
        'dragover dragenter': function (e) {
            $(e.currentTarget).css({ opacity: 0.5 });

            e.preventDefault();
            e.stopPropagation();
        },
        'drop': function (e) {
            onFileDrop(e);

            e.preventDefault();
            e.stopPropagation();
        },
        'dragexit dragend dragleave': function (e) {
            $(e.currentTarget).css({ opacity: 1 });
        }
    });
});

function onFileDrop(evt) {
    $(evt.currentTarget).css({ opacity: 1 });
    try {
        $('#progressbars-container').html('');
        let files = evt.originalEvent.dataTransfer.files;
        for (let i = 0; i < files.length; i++) {
            uploadFile(i, files[i]);
        }
    } catch (e) {
        console.log('onFileDrop error: ');
        console.log(e);
    }
}

function uploadFile(i, file) {
    let name = file.name;
    // create progressbar for each file
    $('#progressbars-container').append($('<span id="spanProgress_' + i + '">' + name + '</span><div class="progress"><div class= "progress-bar" style="width: 0%;" ' +
                                          'aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" id="progressbar_' + i + '">0%</div></div>'));

    //max file chunk size set to 100KB change
    let maxFileSizeKB = 100;
    let fileChunks = [];
    let bufferChunkSizeInBytes = maxFileSizeKB * 1024;
    let currentStreamPosition = 0;
    let endPosition = bufferChunkSizeInBytes;
    let size = file.size;

    while (currentStreamPosition < size) {
        fileChunks.push(file.slice(currentStreamPosition, endPosition));
        currentStreamPosition = endPosition;
        endPosition = currentStreamPosition + bufferChunkSizeInBytes;
    }

    //Append random number to file name to make it unique
    let fileName = Math.random() + "_" + name;
    uploadFileChunk(fileChunks, i, fileName, 1, fileChunks.length);
}

function uploadFileChunk(fileChunks, i, fileName, currentPart, totalPart) {
    let formData = new FormData();
    formData.append('file', fileChunks[currentPart - 1], fileName);

    $.ajax({
        type: "POST",
        url: window.rootUrl + 'FileUpload/UploadFile',
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (totalPart >= currentPart) {
                if (data.status == true) {
                    if (totalPart == currentPart) {
                        //Whole file uploaded
                        //console.log("File upload complete");
                        $('#progressbar_' + i).removeClass('bg-danger').addClass('bg-success');
                    }
                    else {
                        uploadFileChunk(fileChunks, i, fileName, currentPart + 1, totalPart);
                    }
                    //Show uploading progress
                    updateProgress(i, currentPart, totalPart);
                }
                else {
                    //retry message to upload rest of the file
                    // TODO: 
                    updateProgressOnError(i, data.message);
                }
            }
        },
        error: function (a, b, c) {
            //retry message to upload rest of the file
            // TODO: 
            updateProgressOnError(i, b);
        }
    });
}

function updateProgress(i, currentPart, totalPart) {
    let p = (Math.round((currentPart / totalPart) * 100));
    let progress = p + '%';
    $('#progressbar_' + i).width(progress).text(progress).attr("aria-valuenow", p);
}

function updateProgressOnError(i, msg) {
    $('#progressbar_' + i).removeClass('bg-success').addClass('bg-danger');
    $('#spanProgress_' + i).text($('#spanProgress_' + i).text() + ' - ERROR: ' + msg);
}