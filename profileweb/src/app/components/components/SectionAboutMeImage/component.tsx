import { useEffect, useRef } from "react";
import Image from "next/image";
import "./style.scss";

const GearImage1 = "/images/gear/gear_1.svg";
const GearImage2 = "/images/gear/gear_2.svg";
const GearImage3 = "/images/gear/gear_3.svg";
const GearImage4 = "/images/gear/gear_4.svg";
const GearImage5 = "/images/gear/gear_5.svg";
const GearImage6 = "/images/gear/gear_6.svg";
const GearImage7 = "/images/gear/gear_7.svg";

const MaleCertificate = "/images/male_certificate.svg";

export default function SectionHeaderImage() {
  // Referências para as engrenagens
  const gearRefs = useRef<(HTMLImageElement | null)[]>([]);

  const gearSpeeds = [10, 8, 6, 5, 4, 3, 2]; // 3, 2
  const reverseGears = [5,3,1]; 

  const rotateGears = () => {
    gearRefs.current.forEach((gear, index) => {
      if (gear) {
        const rotationSpeed = gearSpeeds[index];
        const rotation = (performance.now() / rotationSpeed) % 360;

        gear.style.transform = `rotate(${rotation}deg)`;

        if (reverseGears.includes(index)) {
          gear.style.transform = `rotate(${-rotation}deg)`; 
        }
      }
    });

    requestAnimationFrame(rotateGears);
  };

  useEffect(() => {
    requestAnimationFrame(rotateGears);

    return () => {
    };
  }, []);

  return (
    <div className="aboutMe_images">
      <div className="content_gear">
        <Image
          src={GearImage1}
          alt="Gear 1"
          id="gear_1"
          width={370}
          height={240}
          ref={(el) => { gearRefs.current[6] = el; }}
        />
        <Image
          src={GearImage2}
          alt="Gear 2"
          id="gear_2"
          width={370}
          height={240}
          ref={(el) => { gearRefs.current[5] = el; }}
        />
        <Image
          src={GearImage3}
          alt="Gear 3"
          id="gear_3"
          width={370}
          height={240}
          ref={(el) => { gearRefs.current[4] = el; }}
        />
        <Image
          src={GearImage4}
          alt="Gear 4"
          id="gear_4"
          width={370}
          height={240}
          ref={(el) => { gearRefs.current[3] = el; }}
        />
        <Image
          src={GearImage5}
          alt="Gear 5"
          id="gear_5"
          width={370}
          height={240}
          ref={(el) => { gearRefs.current[2] = el; }}
        />
        <Image
          src={GearImage6}
          alt="Gear 6"
          id="gear_6"
          width={370}
          height={240}
          ref={(el) => { gearRefs.current[1] = el; }}
        />
        <Image
          src={GearImage7}
          alt="Gear 7"
          id="gear_7"
          width={370}
          height={240}
          ref={(el) => { gearRefs.current[0] = el; }}
        />
      </div>
      <Image
        src={MaleCertificate}
        alt="Male Certificate"
        id="male_certificate"
        width={370}
        height={240}
      />
    </div>
  );
}
