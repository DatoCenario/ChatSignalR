import { Modal } from "antd";
import { useState } from "react";
import ReactCrop, { Crop } from "react-image-crop";
import styled from "styled-components";
import { ImageListImage } from "./uploadedImagesList";

interface ImageEditModalProps {
  image: ImageListImage;
  onComplete?: (newImageUrl: string) => void;
  onCancel?: () => void;
  onClose?: () => void;
}

export const ImageEditModal: React.FC<ImageEditModalProps> = (
  props: ImageEditModalProps
) => {
  const [crop, setCrop] = useState<Crop>({
    width: 100,
    height: 100,
    x: 0,
    y: 0,
    unit: "px",
  });

  const [im, setIm] = useState<HTMLImageElement>(null);

  const handleOk = async () => {
    im && (await makeClientCrop(im));
  };

  const makeClientCrop = async (image: HTMLImageElement) => {
    if (image && crop.width && crop.height) {
      const croppedImageUrl = await getCroppedImg(image, crop);

      props.onComplete && props.onComplete(croppedImageUrl);
    }
  };

  const getCroppedImg = async (
    image: HTMLImageElement,
    crop: Crop
  ): Promise<string> => {
    const canvas = document.createElement("canvas");
    const pixelRatio = window.devicePixelRatio;
    const scaleX = image.naturalWidth / image.width;
    const scaleY = image.naturalHeight / image.height;
    const ctx = canvas.getContext("2d");

    canvas.width = crop.width * pixelRatio * scaleX;
    canvas.height = crop.height * pixelRatio * scaleY;

    ctx.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
    ctx.imageSmoothingQuality = "high";

    ctx.drawImage(
      image,
      crop.x * scaleX,
      crop.y * scaleY,
      crop.width * scaleX,
      crop.height * scaleY,
      0,
      0,
      crop.width * scaleX,
      crop.height * scaleY
    );

    return new Promise((resolve, reject) => {
      var dataURL = canvas.toDataURL("image/png");
      resolve(dataURL);
    });
  };

  return (
    <ModalWrapper>
      <Modal
        open={true}
        title="Edit"
        onOk={handleOk}
        onCancel={props.onCancel}
        afterClose={props.onClose}
      >
        <ReactCrop
          onImageLoaded={(image) => setIm(image)}
          style={{ width: "100%", height: "100%", marginRight: "8px" }}
          src={props.image.url}
          className="crop-container"
          crop={crop}
          onChange={(c) => setCrop(c)}
        ></ReactCrop>
      </Modal>
    </ModalWrapper>
  );
};

const ModalWrapper = styled.div`
  .crop-container {
  }

  .Crop--circle .ReactCrop__crop-selection {
    border-radius: 50%;
  }
`;
