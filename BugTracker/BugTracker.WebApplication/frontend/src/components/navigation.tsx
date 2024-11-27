import { cn } from "@/lib/utils";
import { Settings, UsersIcon } from "lucide-react";
import Link from "next/link";
import {
    GoCheckCircle,
    GoCheckCircleFill,
    GoHome,
    GoHomeFill,
} from "react-icons/go";
const router = [
    {
        label: "Главная",
        href: "/",
        icon: GoHome,
        aciveIcon: GoHomeFill,
    },
    {
        label: "Мои задачи",
        href: "/tasks",
        icon: GoCheckCircle,
        aciveIcon: GoCheckCircleFill,
    },
    {
        label: "Настройки",
        href: "/settings",
        icon: Settings,
        aciveIcon: Settings,
    },
    {
        label: "Команда",
        href: "/members",
        icon: UsersIcon,
        aciveIcon: UsersIcon,
    },
];
export const Navigation = () => {
    return (
        <ul className="flex flex-col">
            {router.map(({ aciveIcon, href, icon, label }) => {
                const isActive = false;
                const Icon = isActive ? aciveIcon : icon;
                return (
                    <Link key={href} href={href}>
                        <div
                            className={cn(
                                "flex items-center gap-2.5 p-2.5 rounded-md font-medium hover:text-primary transition text-neutral-500",
                                isActive && "bg-white shadow-sm hover:opacity-100 text-primary"
                            )}
                        >
                            <Icon className="size-5 text-neutral-500" />
                            {label}
                        </div>
                    </Link>
                );
            })}
        </ul>
    );
};