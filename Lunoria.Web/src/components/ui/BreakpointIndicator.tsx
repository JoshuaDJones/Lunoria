import { useEffect, useState } from "react";

const BreakpointIndicator = () => {
  const [size, setSize] = useState({ width: 0, height: 0 });

  useEffect(() => {
    if (import.meta.env.PROD) return;

    const handler = () =>
      setSize({ width: window.innerWidth, height: window.innerHeight });
    handler();
    window.addEventListener("resize", handler);
    return () => window.removeEventListener("resize", handler);
  }, []);

  if (import.meta.env.PROD) return null;

  return (
    <div className="fixed bottom-2 right-2 z-9999 flex gap-2">
      <div className="bg-black/70 text-white text-xs font-mono px-2 py-1 rounded">
        <span className="sm:hidden">xs</span>
        <span className="hidden sm:inline md:hidden">sm</span>
        <span className="hidden md:inline lg:hidden">md</span>
        <span className="hidden lg:inline xl:hidden">lg</span>
        <span className="hidden xl:inline 2xl:hidden">xl</span>
        <span className="hidden 2xl:inline">2xl</span>
      </div>
      <div className="bg-black/70 text-white text-xs font-mono px-2 py-1 rounded">
        {size.width} x {size.height}
      </div>
    </div>
  );
};

export default BreakpointIndicator;
