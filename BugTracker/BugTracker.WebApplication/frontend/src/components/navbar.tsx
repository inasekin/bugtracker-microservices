"use client";
import { usePathname } from "next/navigation";
import { UserButton } from "@/features/auth/components/user-button";
import { MobileSidebar } from "./mobile-sidebar";

const pathnameMap = {
    tasks: {
        title: "Мои задачи",
        description: "Просмотрите все свои issue здесь",
    },
    projects: {
        title: "Мои проекты",
        description: "Просмотреть issue вашего проекта можно здесь",
    },
};
const defaultMap = {
    title: "Главная",
    description: "Контролируйте все свои проекты и задачи здесь",
};
export const Navbar = () => {
    const pathname = usePathname();
    const parts = pathname.split("/");
    const pathnameKey = parts[3] as keyof typeof pathnameMap;

    const { description, title } = pathnameMap[pathnameKey] || defaultMap;
    return (
        <nav className="pt-4 px-6 flex items-center justify-between">
            <div className="flex-col hidden lg:flex">
                <h1 className="text-2xl font-bold">{title}</h1>
                <p className="text-muted-foreground">{description}</p>
            </div>
            <MobileSidebar />
            <UserButton />
        </nav>
    );
};