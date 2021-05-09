<?php
require_once 'vendor/autoload.php';

use WindowsAzure\Common\ServicesBuilder;
use WindowsAzure\Common\ServiceException;
use WindowsAzure\Blob\BlobRestProxy;
use WindowsAzure\Common\Internal\Resources;
use WindowsAzure\Common\Internal\Validate;
use WindowsAzure\Common\Internal\Utilities;
use WindowsAzure\Common\Internal\Http\HttpClient;
use WindowsAzure\Common\Internal\Filters\DateFilter;
use WindowsAzure\Common\Internal\Filters\HeadersFilter;
use WindowsAzure\Common\Internal\Filters\AuthenticationFilter;
use WindowsAzure\Common\Internal\Filters\WrapFilter;
use WindowsAzure\Common\Internal\InvalidArgumentTypeException;
use WindowsAzure\Common\Internal\Serialization\XmlSerializer;
use WindowsAzure\Common\Internal\Authentication\SharedKeyAuthScheme;
use WindowsAzure\Common\Internal\Authentication\TableSharedKeyLiteAuthScheme;
use WindowsAzure\Common\Internal\StorageServiceSettings;
use WindowsAzure\Common\Internal\ServiceManagementSettings;
use WindowsAzure\Common\Internal\ServiceBusSettings;
use WindowsAzure\Common\Internal\MediaServicesSettings;
use WindowsAzure\Queue\QueueRestProxy;
use WindowsAzure\ServiceBus\ServiceBusRestProxy;
use WindowsAzure\ServiceBus\Internal\WrapRestProxy;
use WindowsAzure\ServiceManagement\ServiceManagementRestProxy;
use WindowsAzure\Table\TableRestProxy;
use WindowsAzure\Table\Internal\AtomReaderWriter;
use WindowsAzure\Table\Internal\MimeReaderWriter;
use WindowsAzure\MediaServices\MediaServicesRestProxy;
use WindowsAzure\Common\Internal\OAuthRestProxy;
use WindowsAzure\Common\Internal\Authentication\OAuthScheme;
$connectionString = "DefaultEndpointsProtocol=https;AccountName=youraccountname;AccountKey=youraccountkey";

// Create blob REST proxy.
$blobRestProxy = ServicesBuilder::getInstance()->createBlobService($connectionString);


try {
    // List blobs.
	error_reporting(0);
	$containerid = $_GET["containerid"];
	$filename = $_GET["filename"];
	$action = $_GET["action"];
	if($action == "list"){
		$blob_list = $blobRestProxy->listBlobs($containerid);
		$blobs = $blob_list->getBlobs();
			$settings = StorageServiceSettings::createFromConnectionString(
				$connectionString
			);

			$uri        = Utilities::tryAddUrlScheme(
				$settings->getBlobEndpointUri()
			);	
			
		$bloblistarr = array();
		foreach($blobs as $blob)
		{
			$myarrblobname = array("imgname" => $blob->getName());
			$bloblistarr[] = $myarrblobname;
			//echo $blob->getName();
		}
		echo json_encode($bloblistarr);
		
	}
	if($action == "delete"){

    // Delete container.
		$blobRestProxy->deleteBlob($containerid, $filename);
		echo "File name: ".$filename." has been delete";
	}
}
catch(ServiceException $e){
    // Handle exception based on error codes and messages.
    // Error codes and messages are here:
    // http://msdn.microsoft.com/library/azure/dd179439.aspx
    $code = $e->getCode();
    $error_message = $e->getMessage();
    echo $code.": ".$error_message."<br />";
}?>