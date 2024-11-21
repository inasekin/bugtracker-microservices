import * as React from "react"
import {Slot} from "@radix-ui/react-slot"
import {cva, type VariantProps} from "class-variance-authority"

import {cn} from "@/lib/utils"

const buttonVariants = cva(
    "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-semibold transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:bg-neutral-100 diabled:from-neutral-100 disbled:to-neutral-100 disabled:text-neutral-300 border border-neutral-200 shadow-sms [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0",
    {
        variants: {
            variant: {
                primary: "bg-[#0052CC] text-[#FFFFFF] shadow hover:bg-[#0747A6]",
                destructive: "bg-[#FF5630] text-[#FFFFFF] shadow-sm hover:bg-[#DE350B]",
                outline:
                    "border-[#DFE1E6] bg-[#FFFFFF] text-[#42526E] shadow-sm hover:bg-[#F4F5F7] hover:text-[#0747A6]",
                secondary:
                    "bg-[#F4F5F7] text-[#42526E] shadow-sm hover:bg-[#DFE1E6]",
                ghost: "hover:bg-[#F4F5F7] hover:text-[#0747A6]",
                link: "text-[#0052CC] underline-offset-4 hover:underline hover:text-[#0747A6]",
            },
            size: {
                default: "h-10 px-4 py-2",
                sm: "h-8 rounded-md px-3",
                xs: "h-8 rounded-md px-2 text-xs",
                lg: "h-12 rounded-md px-8",
                icon: "h-8 w-8",
            },
        },
        defaultVariants: {
            variant: "primary",
            size: "default",
        },
    }
)

export interface ButtonProps
    extends React.ButtonHTMLAttributes<HTMLButtonElement>,
        VariantProps<typeof buttonVariants> {
    asChild?: boolean
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
    ({className, variant, size, asChild = false, ...props}, ref) => {
        const Comp = asChild ? Slot : "button"
        return (
            <Comp
                className={cn(buttonVariants({variant, size, className}))}
                ref={ref}
                {...props}
            />
        )
    }
)
Button.displayName = "Button"

export {Button, buttonVariants}
