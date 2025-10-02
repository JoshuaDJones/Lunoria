import EasyText from "../EasyText";

const FormTitleImage = ({ title }: { title: string }) => {
  return (
    <div className="flex flex-col">
      <EasyText className="self-start text-3xl mb-2">{title}</EasyText>
      <div className="rounded-lg overflow-hidden">
        <img
          src="https://plus.unsplash.com/premium_photo-1673108852141-e8c3c22a4a22?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8Zm9vZHxlbnwwfHwwfHx8MA%3D%3D"
          alt=""
        />
      </div>
    </div>
  );
};

export default FormTitleImage;
