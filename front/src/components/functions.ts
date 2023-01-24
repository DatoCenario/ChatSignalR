export const base64ToUrl = (str: string) => {
  var binaryString = atob(str);

  //Convert binary string to byte array
  var byteArray = new Uint8Array(binaryString.length);
  for (var i = 0; i < binaryString.length; i++) {
    byteArray[i] = binaryString.charCodeAt(i);
  }

  //Create a URL from the byte array
  var url = URL.createObjectURL(new Blob([byteArray], { type: "image/png" }));

  return url;
};
