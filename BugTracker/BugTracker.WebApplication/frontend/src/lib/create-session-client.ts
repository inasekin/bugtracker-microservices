import "server-only";

export const createSessionClient = async () => {
    console.log("Отправка запроса на /api/auth/current...");

    // Запрос к вашему API для проверки текущей сессии
    const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/current`, {
        method: "GET",
        credentials: "include", // Отправляем куки
        headers: {
            "Content-Type": "application/json",
        },
    });

    console.log("Ответ от сервера:", response);

    if (!response.ok) {
        console.error("Ошибка ответа от API:", response.status, response.statusText);
        return null;
    }

    const userData = await response.json();
    console.log("Полученные данные пользователя:", userData);

    return {
        user: userData?.payload, // Например, возвращаем информацию о пользователе
    };
};
