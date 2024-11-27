/* eslint-disable  @typescript-eslint/no-explicit-any */
import { cookies } from "next/headers";
import {AUTH_COOKIE} from "@/features/auth/constants";

export const getCurrent = async (): Promise<any> => {
    try {
        const cookieStore = await cookies();
        const authCookie = cookieStore.get(AUTH_COOKIE);

        if (!authCookie || !authCookie.value) {
            return null;
        }

        // Запрос к вашему API
        const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/current`, {
            method: "GET",
            credentials: "include",
            headers: {
                "Content-Type": "application/json",
                Cookie: `AUTH_COOKIE=${authCookie.value}`, // Передаем куки
            },
        });

        if (!response.ok) {
            throw new Error("Ошибка авторизации");
        }

        const { data } = await response.json();
        return data;
    } catch (error) {
        console.error("Ошибка в getCurrent:", error);
        return null;
    }
};
