# IpfsImageCache

This is simple intermediate file caching server for websites that need to display a large number of images from IPFS.

Problems with IPFS as a source for displaying images in a web application:

- Very slow, even for well compressed and/or reasonably sized images
- Potentially expensive, depending what IPFS gateway you're using
- Images are unpredictable in size and compression

This server aims to help address this by fetching the image once on the first request then caching it permanently:

- The first request for an image will be slow, but subsequently should be on par with any regular web hosted image
- Reduces the number of calls you make to IPFS drastically
- Converts all received images to 400px wide PNG thumnails for consistency
 
Obviously with enough use you may ultimately require a significant amount of storage on your hosting plan.

> The current implementation contains some hardcoded logic specific to my use case such as remote logging to Logtail, mandatory API key protection of the image route, and a 400px hardcoded thumbnail width.

## Technical details

- LibVips native binaries are used for high performance resizing of received images
- Cache operation is sent as a background task over Rebus transport as to not slow client's receipt of image data

## TODO

- Verify that incoming IDs are IPFS and that the received stream is of image type before continuing
- Load test and adjust implementation based on results
- Implement alternative or hybrid caching strategies if required
- Implement configurable cache garbage collection
- See if UseStaticFiles can be configured and is any faster than System.IO.File
- Custom FileResult with permanent Cache-Control header
