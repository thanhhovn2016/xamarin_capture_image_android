var azure = require("azure");
var app = require('express')();
app.enable("jsonp callback");

process.env['AZURE_STORAGE_ACCOUNT'] = "youraccountname";
process.env['AZURE_STORAGE_ACCESS_KEY'] = "youraccountkey";
/*
var azure = require('azure');
var blobs = azure.createBlobService();

blobs.getBlobUrl('teal-test', "",  { AccessPolicy: {
    Start: Date.now(),
    Expiry: azure.date.minutesFromNow(60),
    Permissions: azure.Constants.BlobConstants.SharedAccessPermissions.WRITE
}});

console.log(url);
*/
/*
app.get('/:fileunction(req, res){
	var url = blobService.generateSharedAccessSignature("mycontainerthanhtest", "", {
	AccessPolicy : {
		Permissions : "rwdl",
		Expiry : getDate()
	}});
	console.log(url);
	res.jsonp({url: url.url()});
});
*/
var blobService = azure.createBlobService();
var blobs = [];
function aggregateBlobs(err, result, cb) {
    if (err) {
        cb(er);
    } else {
        blobs = blobs.concat(result.entries);
        if (result.continuationToken !== null) {
            blobService.listBlobsSegmented(
                containerName,
                result.continuationToken,
                aggregateBlobs);
        } else {
            cb(null, blobs);
        }
    }
}


//create container
app.get('/createcon/:userid', function (req, res) {
	var containerName = req.params.userid;
	blobService.createContainerIfNotExists(containerName, function(error, result, response){
	  if(!error){
		  res.send("Container: " + containerName + " has been create");
	  }
	});
});
//end create container
//delete blob
app.get('/deleteblob/:userid/:filename', function (req, res) {
	var containerName = req.params.userid;
	var filename = req.params.filename;
	console.log(filename);
	blobService.deleteBlob(containerName, filename, function(error, response){
	  if(!error){
		res.send("File: " + filename + " has been deleted");
	  }
	});
	
});
//end deleted

app.get('/listblobs/:userid', function (req, res) {
	var userid = req.params.userid;
	var containerName = req.params.userid;
	var myblolbsurlarr = new Array();
blobService.listBlobsSegmented(containerName, null, function(err, result) {
	var urltoken = blobService.generateSharedAccessSignature(containerName, "", {
	AccessPolicy : {
		Permissions : "rwdl",
		Expiry : getDate()
	}});	
    aggregateBlobs(err, result, function(err, blobs) {
        if (err) {
            console.log("Couldn't list blobs");
            console.error(err);
        } else {
            //console.log(blobs);


			for (var i in blobs) {
			  perblob = blobs[i];
			  
			  var mybloburl = "https://spc.blob.core.windows.net/" + containerName + "/" + perblob.name + "?" + urltoken;
			  console.log(mybloburl);
			  var perblobobj = new Object();
			  perblobobj.imgname = mybloburl;
			  perblobobj.imgfilename = perblob.name;
			  myblolbsurlarr.push(perblobobj);
			}
			
			
			
	var blobsjsonString = JSON.stringify(myblolbsurlarr);
	res.send(blobsjsonString);

			
        }
    });
});

});
app.get('/:userid', function (req, res) {
	var userid = req.params.userid;
	blobService.createContainerIfNotExists(userid, function(error){});
	var url = blobService.generateSharedAccessSignature(userid, "", {
	AccessPolicy : {
		Permissions : "rwdl",
		Expiry : getDate()
	}});
	console.log(url);
	//res.jsonp({url: url.url()});
	url = "https://spc.blob.core.windows.net/"+ userid + "?" + url;
	res.send(url);
});

app.get('/gettoken/:userid', function (req, res) {
	var userid = req.params.userid;
	blobService.createContainerIfNotExists(userid, function(error){});
	var url = blobService.generateSharedAccessSignature(userid, "", {
	AccessPolicy : {
		Permissions : "rwdl",
		Expiry : getDate()
	}});
	console.log(url);
	//res.jsonp({url: url.url()});
	//url = "https://spc.blob.core.windows.net/" + "?" + url;
	res.send(url);
});

app.listen(8888);	

function getDate(){
	var date = new Date();
	date.setHours((date).getHours() + 1);
	return date;
}