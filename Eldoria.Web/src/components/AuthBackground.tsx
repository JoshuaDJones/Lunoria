import React, { PropsWithChildren, ReactNode } from "react";

const AuthBackground = ({ children }: PropsWithChildren) => {
  return (
    <div className="bg-gray-50 dark:bg-gray-800 pb-20 h-screen w-screen relative overflow-hidden">
      {/* {nav} */}
      {/* <div className="pb-24 bg-gray-50 px-40 dark:bg-gray-800 h-full relative mx-auto relative overflow-y-auto">
        {children}
      </div> */}

      {/* <img className="absolute right-0 left-0 w-full h-full top-0 bottom-0 z-0" src="./Tavern_night.png" /> */}
      {/* <div className="absolute z-10 top-0 right-0 left-0 bottom-0 p-20 items-center justify-center flex flex-1">
        
        <div className="h-full w-full overflow-y-auto rounded-2xl">
          <div className="m-5"></div>
          {children}
        </div>        
      </div> */}
    </div>
  );
};

export default AuthBackground;
