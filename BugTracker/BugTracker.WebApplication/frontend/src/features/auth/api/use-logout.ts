import { toast } from "sonner";
import { useRouter } from "next/navigation";
import { useMutation, useQueryClient } from "@tanstack/react-query";

export const useLogout = () => {
    const router = useRouter();
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async () => {
            const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/logout`, {
                method: "POST",
                credentials: "include",
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.message || "Ошибка выхода");
            }

            return await response.json();
        },
        onSuccess: () => {
            toast.success("Вы успешно вышли из системы");
            queryClient.invalidateQueries({ queryKey: ["current"] });
            router.push("/sign-in");
        },
        onError: (error: Error) => {
            toast.error(error.message || "Ошибка выхода");
        },
    });
};
