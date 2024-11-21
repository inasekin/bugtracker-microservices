import { useQuery } from "@tanstack/react-query";

export const useCurrent = () => {
    return useQuery({
        queryKey: ["current"],
        queryFn: async () => {
            const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/current`, {
                method: "GET",
                credentials: "include", // Включение куки в запрос
            });

            if (!response.ok) {
                throw new Error("Ошибка при получении текущего пользователя");
            }

            const { data } = await response.json();
            return data;
        },
        staleTime: 5 * 60 * 1000, // Время, в течение которого данные считаются свежими
        retry: false, // Не повторять запрос в случае ошибки
    });
};
