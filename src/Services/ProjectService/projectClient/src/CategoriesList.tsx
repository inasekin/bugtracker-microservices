import * as React from "react"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { CategoryRecord, columns } from "./projectTables/categoriesColumns"
import { DataTable } from "./projectTables/data-table"
 
function getData(): CategoryRecord[] {
  return [
    {
      id: "1",
      category: "Проекты",
      assigned: "Алексей М.",
      command: "Редактировать",
    },
    {
      id: "2",
      category: "Комментарии",
      assigned: "Артём П.",
      command: "Редактировать",
    },
    {
      id: "3",
      category: "Пользователи/Группы",
      assigned: "Иван Н.",
      command: "Редактировать",
    },
    {
      id: "4",
      category: "Уведомления",
      assigned: "Павел С.",
      command: "Редактировать",
    },
    {
      id: "5",
      category: "Файлы",
      assigned: "Вадим C.",
      command: "Редактировать",
    },
    {
      id: "6",
      category: "Задачи",
      assigned: "Александр Н.",
      command: "Редактировать",
    }
  ];
}

export function CategoriesList() {

  const data = getData();

  return (
    <Card>
      <CardHeader>
        <CardTitle>Категории задач</CardTitle>
        <CardDescription className="my-2">Каждый проект разбивается на категории(модули/микросервисы).<br />
	Каждой категории назначается один ответственный на которого отправляются задачи относящиеся к категории.</CardDescription>
      </CardHeader>
      <CardContent>
      	  <div className="container mx-auto ">
      		<DataTable columns={columns} data={data} /> 
          </div>
      </CardContent>
      <CardFooter className="flex justify-between">
        <Button id="addUser">Добавить категорию...</Button>
      </CardFooter>
    </Card>
  )
}