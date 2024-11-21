"use client";
import React from "react";
import Image from "next/image";
import {Button} from "@/components/ui/button";
import Link from "next/link";
import {usePathname} from "next/navigation";

interface AuthLayoutProps {
    children?: React.ReactNode;
}

const AuthLayout = ({children}: AuthLayoutProps) => {
    const pathname = usePathname();
    const isSignIn = pathname === "/sign-in";

    return (
        <main className="bg-neutral-100 min-h-screen">
            <div className="mx-auto max-w-screen-2xl p-4">
                <nav className="flex items-center justify-between">
                    <Image src="/logo.svg" width={150} height={56} alt="logo"/>

                    <Button asChild variant="secondary">
                        <Link href={isSignIn ? "/sign-up" : "/sign-in"}>
                            {isSignIn ? "Создать аккаунт" : "Войти"}
                        </Link>
                    </Button>
                </nav>
            </div>
            <div className="flex flex-col items-center justify-center pt-4 md:py-14">
                {children}
            </div>
        </main>
    );
};

export default AuthLayout;